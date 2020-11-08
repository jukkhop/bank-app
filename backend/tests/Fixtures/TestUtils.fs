namespace Bank

open Bank.BankTransferDb
open Bank.Database
open Bank.TestData
open Bank.Utils

module TestUtils =

  type TestUtils (db: IDatabase) as this =

    member __.InsertOwner (owner: AccountOwner) : unit =
      let sql = @"
        insert into
          account_owner (owner_id, first_name, middle_name, last_name, nationality, date_of_birth)
        values
          (@ownerId, @firstName, @middleName, @lastName, @nationality, @dateOfBirth)"

      let (ownerId, firstName, lastName, nationality, dateOfBirth) =
        match owner with
        | { OwnerId = (OwnerId ownerId)
            FirstName = (FirstName (String50 firstName))
            LastName = (LastName (String50 lastName))
            Nationality = nationality
            DateOfBirth = dateOfBirth } -> (ownerId, firstName, lastName, nationality, dateOfBirth)

      let (MiddleName (String50 middleName)) = owner.MiddleName |> getOrFail "missing middle namae"

      let parms: list<string * obj> = [
        "@ownerId", upcast ownerId
        "@firstName", upcast firstName
        "@middleName", upcast middleName
        "@lastName", upcast lastName
        "@nationality", upcast nationality.ToString()
        "dateOfBirth", upcast dateOfBirth
      ]

      do db.NonQuery sql parms |> ignore

    member __.InsertAccount (account: BankAccount) : unit =
      let sql = @"
        insert into
          bank_account (account_id, owner_id, account_number, balance_eur_cents)
        values
          (@accountId, @ownerId, @accountNumber, @balance)"

      let (accountId, ownerId, accountNumber, balance) =
        match account with
        | { AccountId = (AccountId accountId)
            Owner = { OwnerId = (OwnerId ownerId) }
            AccountNumber = (AccountNumber accountNumber)
            BalanceEurCents = (AccountBalance balance) } -> (accountId, ownerId, accountNumber, balance)

      let parms: list<string * obj> = [
        "@accountId", upcast accountId
        "@ownerId", upcast ownerId
        "@accountNumber", upcast accountNumber
        "@balance", upcast balance
      ]

      do db.NonQuery sql parms |> ignore

    member __.InsertTransfer (transfer: BankTransfer) : unit =
      let sql = @"
        insert into bank_transfer
          (transfer_id, created_at, from_account_id, to_account_id, amount_eur_cents)
        values
          (@transferId, @createdAt, @fromId, @toId, @amount)"

      let (transferId, createdAt, fromId, toId, amount) =
        match transfer with
        | { TransferId = (TransferId transferId)
            CreatedAt = (TransferCreatedAt createdAt)
            FromAccount = { AccountId = (AccountId fromId) }
            ToAccount = { AccountId = (AccountId toId) }
            AmountEurCents = (TransferAmount amount)
          } -> (transferId, createdAt, fromId, toId, amount)

      let parms: list<string * obj> = [
        "@transferId", upcast transferId
        "@createdAt", upcast createdAt
        "@fromId", upcast fromId
        "@toId", upcast toId
        "@amount", upcast amount
      ]

      do db.NonQuery sql parms |> ignore

    member __.GetBalance (AccountId id) : AccountBalance =
      let sql = @"
        select balance_eur_cents
        from bank_account
        where account_id = @id"

      match db.Row sql ["@id", upcast id] (fun read -> read.int64 "balance_eur_cents" |> AccountBalance) with
      | Ok balance -> balance
      | Error ex -> failwith (ex.Message)

    member __.GetTransfer (TransferId id) : BankTransfer =
      let sql = BankTransferDb.SelectSql + " where a.transfer_id = @id"
      match db.Row sql ["@id", upcast id] BankTransferDb.Convert with
      | Ok transfer -> transfer
      | Error ex -> failwith (ex.Message)

    member __.Truncate (tables: string list) : unit =
      do db.NonQuery ("truncate " + (String.concat ", " tables)) [] |> ignore

    member __.CleanAllData() : unit =
      let tables =
        [ "bank_transfer"
          "bank_account"
          "account_owner" ]

      do this.Truncate tables

    member __.InitOwnerData () : unit =
      do this.InsertOwner owner1 |> ignore
      do this.InsertOwner owner2 |> ignore

    member __.InitAccountData () : unit =
      do this.InsertAccount account1 |> ignore
      do this.InsertAccount account2 |> ignore

    member __.InitTransferData () : unit =
      do this.InsertTransfer transfer1 |> ignore
