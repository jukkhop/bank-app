namespace Bank

open Bank.BankAccountDb
open Bank.Database
open Bank.ResultBuilder
open Npgsql.FSharp

module BankTransferDb =

  let private result = ResultBuilder()

  let convert (read: RowReader) : BankTransfer = {
    TransferId = read.int64 "transfer_id" |> TransferId
    Created = read.dateTime "created_at" |> TransferCreatedAt
    FromAccountId = read.int64 "from_account_id" |> AccountId
    ToAccountId = read.int64 "to_account_id" |> AccountId
    AmountEurCents = read.int64 "amount_eur_cents" |> TransferAmount
  }

  let private insertTransfer : Transaction -> AccountId -> AccountId -> TransferAmount -> Result<TransferId, exn> =
    fun tx (AccountId fromId) (AccountId toId) (TransferAmount amount) ->
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

      rowTx tx sql parms (fun read -> read.int64 "transfer_id" |> TransferId)

  let getTransfer (tx: Transaction) (TransferId id) : Result<BankTransfer, exn> =
    let sql = @"
      select
        transfer_id,
        created_at,
        from_account_id,
        to_account_id,
        amount_eur_cents
      from bank_transfer
      where transfer_id = @id"

    rowTx tx sql ["@id", upcast id] convert

  let makeTransfer (fromId: AccountId) (toId: AccountId) (amount: TransferAmount) : Result<BankTransfer, TransferError> =
    inTransaction (fun tx ->
      result {
        let! balance =
          getBalance tx fromId
          |> orFailWith DatabaseError

        let (AccountBalance bal),
            (TransferAmount amt) = balance, amount

        do! (bal >= amt) |> isTrueOrFailWith InsufficientFunds

        return!
          decreaseBalance tx fromId amount
          |> continueWith (fun _ -> increaseBalance tx toId amount)
          |> continueWith (fun _ -> insertTransfer tx fromId toId amount)
          |> continueWith (fun transferId -> getTransfer tx transferId)
          |> orFailWith DatabaseError
      }
    )
