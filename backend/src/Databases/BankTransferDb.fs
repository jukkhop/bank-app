namespace Bank

open Bank.BankAccountDb
open Bank.Database
open Npgsql.FSharp

module BankTransferDb =

  type IBankTransferDb =
    abstract GetAll: unit -> Result<BankTransfer list, exn>
    abstract GetTransfer: Transaction -> TransferId -> Result<BankTransfer, exn>
    abstract InsertTransfer: Transaction -> AccountId -> AccountId -> TransferAmount -> Result<TransferId, exn>

  type BankTransferDb (db: IDatabase) =
    static member Convert (read: RowReader) : BankTransfer =
      { TransferId = read.int64 "transfer_id" |> TransferId
        CreatedAt = read.dateTime "created_at" |> TransferCreatedAt
        FromAccount = BankAccountDb.Convert(read, "from_")
        ToAccount = BankAccountDb.Convert(read, "to_")
        AmountEurCents = read.int64 "amount_eur_cents" |> TransferAmount }

    static member SelectSql: string = @"
      select
        a.transfer_id,
        a.created_at,
        a.amount_eur_cents,
        b.account_id        as from_account_id,
        b.owner_id          as from_owner_id,
        b.account_number    as from_account_number,
        b.balance_eur_cents as from_balance_eur_cents,
        c.first_name        as from_first_name,
        c.middle_name       as from_middle_name,
        c.last_name         as from_last_name,
        c.nationality       as from_nationality,
        c.date_of_birth     as from_date_of_birth,
        d.account_id        as to_account_id,
        d.owner_id          as to_owner_id,
        d.account_number    as to_account_number,
        d.balance_eur_cents as to_balance_eur_cents,
        e.first_name        as to_first_name,
        e.middle_name       as to_middle_name,
        e.last_name         as to_last_name,
        e.nationality       as to_nationality,
        e.date_of_birth     as to_date_of_birth
      from bank_transfer a
      join bank_account b on b.account_id = a.from_account_id
      join account_owner c on c.owner_id = b.owner_id
      join bank_account d on d.account_id = a.to_account_id
      join account_owner e on e.owner_id = d.owner_id"

    interface IBankTransferDb with
      member __.GetAll () : Result<BankTransfer list, exn> =
        db.Query BankTransferDb.SelectSql [] BankTransferDb.Convert

      member __.GetTransfer (tx: Transaction) (TransferId id) : Result<BankTransfer, exn> =
        let sql = BankTransferDb.SelectSql + " where a.transfer_id = @id"
        db.RowTx tx sql ["@id", upcast id] BankTransferDb.Convert

      member __.InsertTransfer (tx: Transaction) (AccountId fromId) (AccountId toId) (TransferAmount amount) : Result<TransferId, exn> =
        let sql = @"
          insert into bank_transfer
            (created_at, from_account_id, to_account_id, amount_eur_cents)
          values
            (now(), @fromId, @toId, @amount)
          returning transfer_id"

        let parms: list<string * obj> = [
          "@fromId", upcast fromId
          "@toId", upcast toId
          "@amount", upcast amount
        ]

        db.RowTx tx sql parms (fun read -> read.int64 "transfer_id" |> TransferId)
