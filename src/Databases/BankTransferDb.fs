namespace Bank

open Bank.Constructors
open Bank.Database
open Npgsql.FSharp

module BankTransferDb =

  let convert (read: RowReader) : BankTransfer = {
    TransferId = read.int64 "transfer_id" |> TransferId
    Created = read.dateTime "created_at" |> TransferCreatedAt
    FromAccountId = read.int64 "from_account_id" |> AccountId
    ToAccountId = read.int64 "to_account_id" |> AccountId
    AmountEurCents = read.int64 "amount_eur_cents" |> TransferAmount
    Result = read.text "result" |> mkTransferResultOrFail
  }

  let private insertTransfer : Transaction -> AccountId -> AccountId -> TransferAmount -> Result<TransferId, exn> =
    fun tx (AccountId fromId) (AccountId toId) (TransferAmount amount) ->
      let sql = sprintf @"
        insert into bank_transfer
          (created_at, from_account_id, to_account_id, amount_eur_cents, result)
        values
          (now(), %i, %i, %i, 'Success')
        returning transfer_id" fromId toId amount
      rowTx tx sql (fun read -> read.int64 "transfer_id" |> TransferId)

  let getTransfer : Transaction -> TransferId -> Result<BankTransfer, exn> =
    fun tx (TransferId id) ->
      let sql = sprintf @"
        select
          transfer_id,
          created_at,
          from_account_id,
          to_account_id,
          amount_eur_cents,
          result
        from bank_transfer
        where transfer_id = %i" id
      rowTx tx sql convert

  let makeTransfer : AccountId -> AccountId -> TransferAmount -> Result<BankTransfer, exn> =
    fun fromId toId amount ->
      inTransaction (fun tx ->
        let balance =
          match BankAccountDb.getBalance tx fromId with
          | Ok balance -> balance
          | Error ex -> raise ex

        let (AccountBalance bal), (TransferAmount amt) = balance, amount

        if bal >= amt then
          BankAccountDb.decreaseBalance tx fromId amount |> ignore
          BankAccountDb.increaseBalance tx toId amount |> ignore

          let transferId =
            match insertTransfer tx fromId toId amount with
            | Ok transferId -> transferId
            | Error ex -> raise ex

          getTransfer tx transferId

        else Error <| failwith "foo"
      )
