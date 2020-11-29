namespace Bank

open Amazon.Lambda.APIGatewayEvents
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

module CreateBankTransferSpec =

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

  let private validDto: CreateBankTransferDto =
    { FromAccountId = Some accountId1Int
      ToAccountId = Some accountId2Int
      AmountEurCents = Some transfer1AmountInt }

  let private emptyDto: CreateBankTransferDto =
    { FromAccountId = None
      ToAccountId = None
      AmountEurCents = None }

  let private createTransfer<'a> (body: string) (db: IDatabase) (accountDb: IBankAccountDb) (transferDb: IBankTransferDb) =
    let request = APIGatewayProxyRequest(Body = body)
    let service = TransferService(db, accountDb, transferDb) :> ITransferService
    let response = CreateBankTransfer.impl request service
    (response.StatusCode, response.Body)

  let private getOkOrFail (res: Result<'a, 'b>) =
    match res with
    | Ok x -> x
    | Error _ -> failwith "got an Error, expected Ok"

  [<Fact>]
  let ``transfer funds from one account to another`` () =
    do setupDb()

    let (status, body) = createTransfer (Json.serialize validDto) db accountDb transferDb
    let responseDto = Json.deserialize<CreateBankTransferResponseDto> body |> getOkOrFail
    let returnedTransfer = responseDto.Transfer

    do status |> should equal 200
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
  let ``return with a parse error if the payload is incorrecly formatted`` () =
    let (dbMock, _) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>().Create()
    let transferDbMock = Mock<IBankTransferDb>().Create()

    let (status, body) = createTransfer "foo" dbMock accountDbMock transferDbMock
    let response = Json.deserialize<GenericErrorBody> body |> getOkOrFail

    do status |> should equal 400
    do response.Message |> should startWith "Invalid JSON"

  [<Fact>]
  let ``return with a validation error if the payload does not pass validation`` () =
    let (dbMock, _) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>().Create()
    let transferDbMock = Mock<IBankTransferDb>().Create()

    let (status, body) = createTransfer (Json.serialize emptyDto) dbMock accountDbMock transferDbMock
    let response = Json.deserialize<ValidationErrorBody> body |> getOkOrFail

    do status |> should equal 400
    do response.ValidationErrors |> should not' (be Empty)

  [<Fact>]
  let ``return with an error if there are insufficient funds on the source account`` () =
    let (dbMock, txMock) = setupDbMock()
    let insufficientAmount = transfer1AmountInt - 1L

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@
        mock.AccountIdExists accountId1Int --> Ok true
        mock.AccountIdExists accountId2Int --> Ok true
        mock.GetBalance txMock accountId1 --> Ok (AccountBalance insufficientAmount)
      @>
    )

    let transferDbMock = Mock<IBankTransferDb>().Create()

    let (status, body) = createTransfer (Json.serialize validDto) dbMock accountDbMock transferDbMock
    let response = Json.deserialize<ErrorBody> body |> getOkOrFail

    do status |> should equal 409
    do response |> should equal { Reason = "INSUFFICIENT_FUNDS"; Message = "Insufficient funds on the account" }

  [<Fact>]
  let ``return with an error if there is a database error during the transfer`` () =
    let (dbMock, txMock) = setupDbMock()

    let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
      <@
        mock.AccountIdExists accountId1Int --> Ok true
        mock.AccountIdExists accountId2Int --> Ok true
        mock.GetBalance txMock accountId1 --> Error (Failure "some error")
      @>
    )

    let transferDbMock = Mock<IBankTransferDb>().Create()

    let (status, body) = createTransfer (Json.serialize validDto) dbMock accountDbMock transferDbMock
    let response = Json.deserialize<ErrorBody> body |> getOkOrFail

    do status |> should equal 500
    do response |> should equal { Reason = "DATABASE_ERROR"; Message = "some error" }
