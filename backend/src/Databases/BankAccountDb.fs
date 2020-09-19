namespace Bank

open Bank.AccountNumberUtils
open Bank.Database
open Npgsql.FSharp

module BankAccountDb =

  let convert (read: RowReader) : BankAccount = {
    AccountId = read.int64 "account_id" |> AccountId
    Owner = AccountOwnerDb.convert read
    AccountNumber = read.text "account_number" |> mkAccountNumberOrFail
    BalanceEurCents = read.int64 "balance_eur_cents" |> AccountBalance
  }

  let getAll () : Result<BankAccount list, exn> =
    let sql = @"
      select
        a.account_id,
        a.account_number,
        a.balance_eur_cents,
        o.owner_id,
        o.first_name,
        o.middle_name,
        o.last_name,
        o.nationality,
        o.date_of_birth
      from bank_account a
      join account_owner o using (owner_id)"
    query sql [] convert

  let increaseBalance (tx: Transaction) (AccountId id) (TransferAmount amount) : Result<unit, exn> =
    let sql = @"
      update bank_account
      set balance_eur_cents = balance_eur_cents + @amount
      where account_id = @id"

    nonQueryTx tx sql [ "@amount", upcast amount
                        "@id", upcast id ]

  let decreaseBalance (tx: Transaction) (id: AccountId) (TransferAmount amount) : Result<unit, exn> =
    increaseBalance tx id (amount * -1L |> TransferAmount)

  let getBalance (tx: Transaction) (AccountId id) : Result<AccountBalance, exn> =
    let sql = @"
      select balance_eur_cents
      from bank_account
      where account_id = @id"
    rowTx tx sql ["@id", upcast id] (fun read -> read.int64 "balance_eur_cents" |> AccountBalance)
