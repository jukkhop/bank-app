namespace Bank

open Bank.BankAccountDb
open Bank.BankTransferDb
open Bank.Database

module TransferService =

  type ITransferService =
    abstract AccountDb: IBankAccountDb
    abstract TransferDb: IBankTransferDb
    abstract MakeTransfer: AccountId -> AccountId -> TransferAmount -> Result<BankTransfer, TransferError>

  type TransferService (db: IDatabase, accountDb: IBankAccountDb, transferDb: IBankTransferDb) =
    interface ITransferService with
      member __.AccountDb = accountDb
      member __.TransferDb = transferDb

      member __.MakeTransfer (fromId: AccountId) (toId: AccountId) (amount: TransferAmount) : Result<BankTransfer, TransferError> =
        let txResult = db.InTransaction (fun tx ->
          result {
            let! balance =
              accountDb.GetBalance tx fromId |> orFailWithCase DatabaseError

            let (AccountBalance bal),
                (TransferAmount amt) = balance, amount

            do! (bal >= amt) |> isTrueOrFailWith InsufficientFunds

            return!
              accountDb.DecreaseBalance tx fromId amount
              |> continueWith (fun _ -> accountDb.IncreaseBalance tx toId amount)
              |> continueWith (fun _ -> transferDb.InsertTransfer tx fromId toId amount)
              |> continueWith (fun transferId -> transferDb.GetTransfer tx transferId)
              |> orFailWithCase DatabaseError
          }
        )
        match txResult with
        | Ok result -> result
        | Error ex -> Error (DatabaseError ex.Message)
