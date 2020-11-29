namespace Bank

open Bank.BankAccountDb
open Bank.BankTransferDb
open Bank.Config
open Bank.Context
open Bank.Database
open Bank.TestData
open Bank.TestDbUtils
open Bank.TransferService
open Foq
open FsUnit.Xunit
open System.Data.Common
open Xunit

module TransferServiceSpec =

  type private TxFun = DbTransaction -> Result<BankTransfer, TransferError>

  let private context = Context (defaultConfig)
  let private db = context.Db
  let private accountDb = context.AccountDb
  let private transferDb = context.TransferDb
  let private utils = TestDbUtils (context.Db)

  let private setupDb () : unit =
    do utils.CleanAllData()
    do utils.InitOwnerData()
    do utils.InitAccountData()

  let private setupDbMock () : IDatabase * DbTransaction =
    let tx = Mock<DbTransaction>().Create()

    let db =
      Mock<IDatabase>()
        .Setup(fun mock -> <@ mock.InTransaction (any()) @>)
        .Calls<TxFun>(fun fn -> Ok (fn tx))
        .Create()

    (db, tx)

  let private getOkOrFail (res: Result<'a, 'b>) =
    match res with
    | Ok x -> x
    | Error _ -> failwith "got an Error, expected Ok"

  let private getErrorOrFail (res: Result<'a, 'b>) =
    match res with
    | Ok _ -> failwith "got an Ok, expected Error"
    | Error err -> err

  [<Fact>]
  let ``transfer funds from one account to another`` () =
    do setupDb()

    let service = TransferService(db, accountDb, transferDb) :> ITransferService
    let returnedTransfer = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getOkOrFail

    do utils.GetBalance accountId1 |> should equal (AccountBalance (account1BalanceInt - transfer1AmountInt))
    do utils.GetBalance accountId2 |> should equal (AccountBalance (account2BalanceInt + transfer1AmountInt))

    let extract (transfer: BankTransfer) : AccountId * AccountId * TransferAmount =
      match transfer with
      | { FromAccount = { AccountId = fromId }
          ToAccount = { AccountId = toId }
          AmountEurCents = amount } -> (fromId, toId, amount)

    let actualTransfer = returnedTransfer.TransferId |> utils.GetTransfer |> extract

    do actualTransfer |> should equal (extract returnedTransfer)
    do actualTransfer |> should equal (extract transfer1)

  [<Fact>]
  let ``use the same transaction for each database operation`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@
        mock.GetBalance txMock accountId1 --> Ok account1Balance
        mock.DecreaseBalance txMock accountId1 transfer1Amount --> Ok ()
        mock.IncreaseBalance txMock accountId2 transfer1Amount --> Ok ()
      @>
    )

    let transferDbMock = Mock<IBankTransferDb>.With(fun mock ->
      <@
        mock.InsertTransfer txMock accountId1 accountId2 transfer1Amount --> Ok transferId1
        mock.GetTransfer txMock transferId1 --> Ok transfer1
      @>
    )

    let service = TransferService (dbMock, accountDbMock, transferDbMock) :> ITransferService
    let returnedTransfer = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getOkOrFail

    do Mock.Verify (<@ accountDbMock.GetBalance txMock accountId1 @>, once)
    do Mock.Verify (<@ accountDbMock.DecreaseBalance txMock accountId1 transfer1Amount @>, once)
    do Mock.Verify (<@ accountDbMock.IncreaseBalance txMock accountId2 transfer1Amount @>, once)
    do Mock.Verify (<@ transferDbMock.InsertTransfer txMock accountId1 accountId2 transfer1Amount @>, once)
    do Mock.Verify (<@ transferDbMock.GetTransfer txMock returnedTransfer.TransferId @>, once)

  [<Fact>]
  let ``roll back the transaction in case of an error`` () =
    do setupDb()

    let transferDbMock = Mock<IBankTransferDb>.With(fun mock ->
      <@
        mock.InsertTransfer (any()) (any()) (any()) (any()) --> Error (Failure "some error")
      @>
    )

    let service = TransferService (db, accountDb, transferDbMock) :> ITransferService
    let error = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getErrorOrFail

    do error |> should equal (DatabaseError "some error")
    do utils.GetBalance accountId1 |> should equal account1Balance
    do utils.GetBalance accountId2 |> should equal account2Balance
    do Mock.Verify (<@ transferDbMock.InsertTransfer (any()) (any()) (any()) (any()) @>, once)

  [<Fact>]
  let ``return an error when checking the balance of the source account fails`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@ mock.GetBalance txMock accountId1 --> Error (Failure "some error") @>
    )

    let transferDbMock = Mock<IBankTransferDb>().Create()
    let service = TransferService(dbMock, accountDbMock, transferDbMock) :> ITransferService
    let error = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getErrorOrFail

    do error |> should equal (DatabaseError "some error")
    do Mock.Verify (<@ accountDbMock.GetBalance txMock accountId1 @>, once)

  [<Fact>]
  let ``return an error when there are insufficient funds on the source account`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@ mock.GetBalance txMock accountId1 --> Ok (AccountBalance 0L) @>
    )

    let transferDbMock = Mock<IBankTransferDb>().Create()
    let service = TransferService(dbMock, accountDbMock, transferDbMock) :> ITransferService
    let error = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getErrorOrFail

    do error |> should equal InsufficientFunds
    do Mock.Verify (<@ accountDbMock.GetBalance txMock accountId1 @>, once)

  [<Fact>]
  let ``return an error when decreasing the balance of the source account fails`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@
        mock.GetBalance txMock accountId1 --> Ok account1Balance
        mock.DecreaseBalance txMock accountId1 transfer1Amount --> Error (Failure "some error")
      @>
    )

    let transferDbMock = Mock<IBankTransferDb>().Create()
    let service = TransferService(dbMock, accountDbMock, transferDbMock) :> ITransferService
    let error = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getErrorOrFail

    do error |> should equal (DatabaseError "some error")

    do Mock.Verify (<@ accountDbMock.GetBalance txMock accountId1 @>, once)
    do Mock.Verify (<@ accountDbMock.DecreaseBalance txMock accountId1 transfer1Amount @>, once)

  [<Fact>]
  let ``return an error when increasing the balance of the target account fails`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@
        mock.GetBalance txMock accountId1 --> Ok account1Balance
        mock.DecreaseBalance txMock accountId1 transfer1Amount --> Ok ()
        mock.IncreaseBalance txMock accountId2 transfer1Amount --> Error (Failure "some error")
      @>
    )

    let transferDbMock = Mock<IBankTransferDb>().Create()
    let service = TransferService(dbMock, accountDbMock, transferDbMock) :> ITransferService
    let error = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getErrorOrFail

    do error |> should equal (DatabaseError "some error")

    do Mock.Verify (<@ accountDbMock.GetBalance txMock accountId1 @>, once)
    do Mock.Verify (<@ accountDbMock.DecreaseBalance txMock accountId1 transfer1Amount @>, once)
    do Mock.Verify (<@ accountDbMock.IncreaseBalance txMock accountId2 transfer1Amount @>, once)

  [<Fact>]
  let ``return an error when inserting the transfer fails`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@
        mock.GetBalance txMock accountId1 --> Ok account1Balance
        mock.DecreaseBalance txMock accountId1 transfer1Amount --> Ok ()
        mock.IncreaseBalance txMock accountId2 transfer1Amount --> Ok ()
      @>
    )

    let transferDbMock = Mock<IBankTransferDb>.With(fun mock ->
      <@
        mock.InsertTransfer txMock accountId1 accountId2 transfer1Amount --> Error (Failure "some error")
      @>
    )

    let service = TransferService(dbMock, accountDbMock, transferDbMock) :> ITransferService
    let error = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getErrorOrFail

    do error |> should equal (DatabaseError "some error")

    do Mock.Verify (<@ accountDbMock.GetBalance txMock accountId1 @>, once)
    do Mock.Verify (<@ accountDbMock.DecreaseBalance txMock accountId1 transfer1Amount @>, once)
    do Mock.Verify (<@ accountDbMock.IncreaseBalance txMock accountId2 transfer1Amount @>, once)
    do Mock.Verify (<@ transferDbMock.InsertTransfer txMock accountId1 accountId2 transfer1Amount @>, once)

  [<Fact>]
  let ``return an error when getting the transfer fails`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@
        mock.GetBalance txMock accountId1 --> Ok account1Balance
        mock.DecreaseBalance txMock accountId1 transfer1Amount --> Ok ()
        mock.IncreaseBalance txMock accountId2 transfer1Amount --> Ok ()
      @>
    )

    let transferDbMock = Mock<IBankTransferDb>.With(fun mock ->
      <@
        mock.InsertTransfer txMock accountId1 accountId2 transfer1Amount --> Ok transferId1
        mock.GetTransfer txMock transferId1 --> Error (Failure "some error")
      @>
    )

    let service = TransferService(dbMock, accountDbMock, transferDbMock) :> ITransferService
    let error = service.MakeTransfer accountId1 accountId2 transfer1Amount |> getErrorOrFail

    do error |> should equal (DatabaseError "some error")

    do Mock.Verify (<@ accountDbMock.GetBalance txMock accountId1 @>, once)
    do Mock.Verify (<@ accountDbMock.DecreaseBalance txMock accountId1 transfer1Amount @>, once)
    do Mock.Verify (<@ accountDbMock.IncreaseBalance txMock accountId2 transfer1Amount @>, once)
    do Mock.Verify (<@ transferDbMock.InsertTransfer txMock accountId1 accountId2 transfer1Amount @>, once)
    do Mock.Verify (<@ transferDbMock.GetTransfer txMock transferId1 @>, once)
