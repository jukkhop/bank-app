namespace Bank

open Bank.Database
open Bank.NationalityUtils
open Bank.StringUtils
open Bank.DateTimeUtils
open Npgsql.FSharp

module AccountOwnerDb =

  let convert (read: RowReader) : AccountOwner = {
    OwnerId = read.int64 "owner_id" |> OwnerId
    FirstName = read.text "first_name" |> mkString50OrFail |> FirstName
    MiddleName = read.textOrNone "middle_name" |> Option.map (mkString50OrFail >> MiddleName)
    LastName = read.text "last_name" |> mkString50OrFail |> LastName
    Nationality = read.text "nationality" |> mkNationalityOrFail
    DateOfBirth = read.date "date_of_birth" |> mkDateTime
  }

  let getAll () : Result<AccountOwner list, exn> =
    let sql = @"
      select
        owner_id,
        first_name,
        middle_name,
        last_name,
        nationality,
        date_of_birth
      from account_owner"
    query sql [] convert
