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
      let sql = @"
        insert into bank_transfer
          (created_at, from_account_id, to_account_id, amount_eur_cents, result)
        values
          (now(), @fromId, @toId, @amount, 'Success')
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
        amount_eur_cents,
        result
      from bank_transfer
      where transfer_id = @id"
    rowTx tx sql ["@id", upcast id] convert

  let makeTransfer (fromId: AccountId) (toId: AccountId) (amount: TransferAmount) : Result<BankTransfer, exn> =
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


