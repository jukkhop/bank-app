namespace Bank

open Bank.Constructors
open Bank.Database
open Npgsql.FSharp

module BankAccountDb =

  let convert (read: RowReader) : BankAccount = {
    AccountId = read.int64 "account_id" |> AccountId
    Owner = AccountOwnerDb.convert read
    AccountNumber = read.text "account_number" |> mkAccountNumberOrFail
    BalanceEurCents = read.int64 "balance_eur_cents" |> AccountBalance
  }

  let getAll: Result<BankAccount list, exn> =

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

    query sql convert
