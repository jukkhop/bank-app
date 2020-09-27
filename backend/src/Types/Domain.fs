namespace Bank

open FSharp.Json
open System

type String50 = String50 of string

type AccountBalance = AccountBalance of int64
type AccountId = AccountId of int64
type AccountNumber = AccountNumber of string
type FirstName = FirstName of String50
type LastName = LastName of String50
type MiddleName = MiddleName of String50
type OwnerId = OwnerId of int64
type TransferAmount = TransferAmount of int64
type TransferId = TransferId of int64
type TransferCreatedAt = TransferCreatedAt of DateTime

type Nationality =
  | [<JsonUnionCase(Case="Austria")>] Austria
  | [<JsonUnionCase(Case="Denmark")>] Denmark
  | [<JsonUnionCase(Case="Netherlands")>] Netherlands
  | [<JsonUnionCase(Case="Sweden")>] Sweden

type TransferError =
  | InsufficientFunds
  | DatabaseError of string
  | OtherError of string

type AccountOwner = {
  OwnerId: OwnerId
  FirstName: FirstName
  MiddleName: MiddleName option
  LastName: LastName
  Nationality: Nationality
  DateOfBirth: DateTime
}

type BankAccount = {
  AccountId: AccountId
  Owner: AccountOwner
  AccountNumber: AccountNumber
  BalanceEurCents: AccountBalance
}

type BankTransfer = {
  TransferId: TransferId
  CreatedAt: TransferCreatedAt
  FromAccount: BankAccount
  ToAccount: BankAccount
  AmountEurCents: TransferAmount
}
