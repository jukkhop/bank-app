namespace Bank

open Bank.Database
open Bank.NationalityUtils
open Bank.StringUtils
open Bank.DateTimeUtils
open Npgsql.FSharp
open System

module AccountOwnerDb =

  type IAccountOwnerDb =
    abstract GetAll: unit -> Result<AccountOwner list, exn>

  type AccountOwnerDb() =
    static member Convert (read: RowReader, ?columnPrefix: string) : AccountOwner =
      let prefix = defaultArg columnPrefix String.Empty
      let column colName = prefix + colName
      { OwnerId = read.int64 (column "owner_id") |> OwnerId
        FirstName = read.text (column "first_name") |> mkString50OrFail |> FirstName
        MiddleName = read.textOrNone (column "middle_name") |> Option.map (mkString50OrFail >> MiddleName)
        LastName = read.text (column "last_name") |> mkString50OrFail |> LastName
        Nationality = read.text (column "nationality") |> mkNationalityOrFail
        DateOfBirth = read.date (column "date_of_birth") |> mkDateTime }

    interface IAccountOwnerDb with
      member __.GetAll  () : Result<AccountOwner list, exn> =
        let sql = @"
          select
            owner_id,
            first_name,
            middle_name,
            last_name,
            nationality,
            date_of_birth
          from account_owner"

        query sql [] AccountOwnerDb.Convert
