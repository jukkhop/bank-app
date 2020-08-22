namespace Bank

open Bank
open Bank.Constructors
open Bank.Database
open Npgsql.FSharp

module AccountOwnerDb =

  let getAll: AccountOwner list =

    let sql = @"
      select
        first_name,
        middle_name,
        last_name,
        nationality,
        date_of_birth
      from account_owner"

    let readFn (read: RowReader): AccountOwner = {
      FirstName = read.text "first_name" |> mkString50OrFail
      MiddleName = read.textOrNone "middle_name" |> Option.map mkString50OrFail
      LastName = read.text "last_name" |> mkString50OrFail
      Nationality = read.text "nationality" |> mkNationalityOrFail
      DateOfBirth = read.date "date_of_birth" |> mkDateTime
    }

    match query sql readFn with
     | Ok rows -> rows
     | Error err -> raise err
