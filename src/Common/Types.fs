namespace Bank

open System

type String50 = String50 of string
type AccountNumber = AccountNumber of string
type AccountBalance = AccountBalance of uint64
type TransferAmount = TransferAmount of uint64

type Nationality =
  | Austria
  | Denmark
  | Netherlands
  | Sweden

type AccountOwner = {
  FirstName: String50
  MiddleName: String50 option
  LastName: String50
  Nationality: Nationality
  DateOfBirth: DateTime
}

type BankAccount = {
  Owner: AccountOwner
  AccountNumber: AccountNumber
  BalanceEurCents: AccountBalance
}

type TransferResult =
  | Success of decimal
  | InsufficientFunds of decimal
  | NetworkError
  | OtherError of string

type BankTransfer = {
  TransferId: String50
  Created: DateTime
  FromAccount: AccountNumber
  ToAccount: AccountNumber
  AmountEurCents: TransferAmount
  Result: TransferResult
}
