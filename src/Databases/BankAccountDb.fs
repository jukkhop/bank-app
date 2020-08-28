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

  let increaseBalance : Transaction -> AccountId -> TransferAmount -> Result<int, exn> =
    fun tx (AccountId id) (TransferAmount amount) ->
      let sql = sprintf @"
        update bank_account
        set balance_eur_cents = balance_eur_cents + %i
        where account_id = %i" amount id
      nonQueryTx tx sql

  let decreaseBalance : Transaction -> AccountId -> TransferAmount -> Result<int, exn> =
    fun tx id (TransferAmount amount) ->
      increaseBalance tx id (TransferAmount <| amount * -1L)

  let getBalance : Transaction -> AccountId -> Result<AccountBalance, exn> =
    fun tx (AccountId id) ->
      let sql = sprintf @"
        select balance_eur_cents
        from bank_account
        where account_id = %i" id
      rowTx tx sql (fun read -> read.int64 "balance_eur_cents" |> AccountBalance)
