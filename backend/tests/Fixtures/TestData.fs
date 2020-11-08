namespace Bank

open Bank
open System

module TestData =

  let ownerId1 = OwnerId 1L
  let ownerId2 = OwnerId 2L
  let accountId1Int = 3L
  let accountId2Int = 4L
  let accountId1 = AccountId accountId1Int
  let accountId2 = AccountId accountId2Int
  let account1BalanceInt = 5000L
  let account2BalanceInt = 0L
  let account1Balance = AccountBalance account1BalanceInt
  let account2Balance = AccountBalance account2BalanceInt
  let transferId1 = TransferId 5L
  let transfer1AmountInt = 1000L
  let transfer1Amount = TransferAmount transfer1AmountInt

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

  let transfer1: BankTransfer = {
    TransferId = transferId1
    CreatedAt = TransferCreatedAt DateTime.Now
    FromAccount = account1
    ToAccount = account2
    AmountEurCents = transfer1Amount
  }
