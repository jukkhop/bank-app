namespace Bank

open Bank.AccountOwnerDb
open Bank.Database
open Npgsql.FSharp
open System

module BankAccountDb =

  type IBankAccountDb =
    abstract AccountIdExists: int64 -> Result<bool, exn>
    abstract DecreaseBalance: Transaction -> AccountId -> TransferAmount -> Result<unit, exn>
    abstract GetAll: unit -> Result<BankAccount list, exn>
    abstract GetBalance: Transaction -> AccountId -> Result<AccountBalance, exn>
    abstract IncreaseBalance: Transaction -> AccountId -> TransferAmount -> Result<unit, exn>

  type BankAccountDb (db: IDatabase) as this =
    static member Convert (read: RowReader, ?columnPrefix: string) : BankAccount =
      let prefix = defaultArg columnPrefix String.Empty
      let column colName = prefix + colName
      { AccountId = read.int64 (column "account_id") |> AccountId
        Owner = AccountOwnerDb.Convert(read, prefix)
        AccountNumber = read.text (column "account_number") |> AccountNumber
        BalanceEurCents = read.int64 (column "balance_eur_cents") |> AccountBalance }

    interface IBankAccountDb with
      member __.AccountIdExists (id: int64) : Result<bool, exn> =
        let sql = "select exists (select 1 from bank_account where account_id = @id)"
        db.Row sql ["@id", upcast id] (fun read -> read.bool "exists")

      member __.DecreaseBalance (tx: Transaction) (id: AccountId) (TransferAmount amount) : Result<unit, exn> =
        let me = this :> IBankAccountDb
        me.IncreaseBalance tx id (amount * -1L |> TransferAmount)

      member __.GetAll () : Result<BankAccount list, exn> =
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

        db.Query sql [] BankAccountDb.Convert

      member __.GetBalance (tx: Transaction) (AccountId id) : Result<AccountBalance, exn> =
        let sql = @"
          select balance_eur_cents
          from bank_account
          where account_id = @id"

        db.RowTx tx sql ["@id", upcast id] (fun read -> read.int64 "balance_eur_cents" |> AccountBalance)

      member __.IncreaseBalance (tx: Transaction) (AccountId id) (TransferAmount amount) : Result<unit, exn> =
        let sql = @"
          update bank_account
          set balance_eur_cents = balance_eur_cents + @amount
          where account_id = @id"

        db.NonQueryTx tx sql [ "@amount", upcast amount
                               "@id", upcast id ]
