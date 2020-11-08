module Tests

open Amazon.Lambda.APIGatewayEvents
open Bank
open Bank.BankAccountDb
open Bank.BankTransferDb
open Bank.TransferService
open Foq
open FsUnit.Xunit
open System
open Xunit

let ownerId1 = OwnerId 1L
let ownerId2 = OwnerId 2L
let accountId1Int = 3L
let accountId2Int = 4L
let accountId1 = AccountId accountId1Int
let accountId2 = AccountId accountId2Int
let account1Balance = AccountBalance 5000L
let account2Balance = AccountBalance 0L
let transferId = TransferId 5L
let transferAmountInt = 1000L
let transferAmount = TransferAmount transferAmountInt

let owner1: AccountOwner = {
  OwnerId = ownerId1
  FirstName = FirstName (String50 "First name")
  MiddleName = Some <| MiddleName (String50 "Middle name")
  LastName = LastName (String50 "Last name")
  Nationality = Austria
  DateOfBirth = DateTime.Now
}

let owner2: AccountOwner = {
  OwnerId = ownerId2
  FirstName = FirstName (String50 "First name")
  MiddleName = Some <| MiddleName (String50 "Middle name")
  LastName = LastName (String50 "Last name")
  Nationality = Sweden
  DateOfBirth = DateTime.Now
}

let account1: BankAccount = {
  AccountId = accountId1
  Owner = owner1
  AccountNumber = AccountNumber "3478754321234567"
  BalanceEurCents = account1Balance
}

let account2: BankAccount = {
  AccountId = accountId2
  Owner = owner2
  AccountNumber = AccountNumber "3478754321234567"
  BalanceEurCents = account2Balance
}

let transfer: BankTransfer = {
  TransferId = transferId
  CreatedAt = TransferCreatedAt DateTime.Now
  FromAccount = account1
  ToAccount = account2
  AmountEurCents = transferAmount
}

[<Fact>]
let ``foobar`` () =

  let accountDb = Mock<IBankAccountDb>.With(fun mock ->
    <@
      mock.AccountIdExists (any()) --> Ok true
      mock.DecreaseBalance (any()) accountId1 transferAmount --> Ok ()
      mock.GetBalance (any()) accountId1 --> Ok account1Balance
      mock.GetBalance (any()) accountId2 --> Ok account2Balance
      mock.IncreaseBalance (any()) accountId2 transferAmount --> Ok ()
    @>
  )

  let transferDb = Mock<IBankTransferDb>.With(fun mock ->
    <@
      mock.GetTransfer (any()) transferId --> Ok transfer
      mock.InsertTransfer (any()) accountId1 accountId2 transferAmount --> Ok transferId
    @>
  )

  let request: CreateBankTransferDto = {
    FromAccountId = Some accountId1Int
    ToAccountId = Some accountId2Int
    AmountEurCents = Some transferAmountInt
  }

  let service = TransferService(accountDb, transferDb)
  let request = APIGatewayProxyRequest(Body = Json.serialize request)
  let response = CreateBankTransfer.impl request service
  let body = Json.deserialize<CreateBankTransferResponseDto> response.Body

  match body with
  | Ok res -> res.Transfer |> should equal transfer
  | Error ex -> raise ex

  Mock.Verify (<@ accountDb.AccountIdExists accountId1Int @>, once)
  Mock.Verify (<@ accountDb.AccountIdExists accountId2Int @>, once)
  Mock.Verify (<@ accountDb.GetBalance (any()) accountId1 @>, once)
  Mock.Verify (<@ accountDb.DecreaseBalance (any()) accountId1 transferAmount @>, once)
  Mock.Verify (<@ accountDb.IncreaseBalance (any()) accountId2 transferAmount @>, once)
  Mock.Verify (<@ transferDb.InsertTransfer(any()) accountId1 accountId2 transferAmount @>, once)
  Mock.Verify (<@ transferDb.GetTransfer(any()) transferId @>, once)
