namespace Bank

open Bank.Operators
open Bank.StringUtils

module Validators =

  let private mkError (field: string) (message: string) : ValidationError =
    { Field = camelify field
      Message = message }

  let requiredValidator (field: string) (value: obj option) : ValidationError option =
    match value with
    | Some _ -> None
    | None -> Some <| mkError field "Value is required"

  let rangeValidator (min: int64, max: int64) (field: string) (opt: obj option) : ValidationError option =
    option {
      let! value = opt
      return!
        match unbox<int64> value with
        | x when x >=< (min, max) -> None
        | _ -> sprintf "Value is not within range, min: %i, max: %i" min max |> mkError field |> Some
    }

  let accountIdExistsValidator (exists: int64 -> Result<bool, exn>) (field: string) (opt: obj option) : ValidationError option =
    option {
      let! value = opt
      let id = unbox<int64> value
      return!
        match exists id with
        | Ok true -> None
        | Ok false -> sprintf "Bank account with account ID %i not found" id |> mkError field |> Some
        | Error ex -> failwithf "Database error while checking account ID: %s" ex.Message
    }

  let differentAccountsValidator (data: CreateBankTransferDto) : ValidationError list =
    let sameAccountError =
      { Field = "(fromAccountId,toAccountId)"
        Message = "Source and destination accounts cannot be the same account" }
    match (data.FromAccountId, data.ToAccountId) with
    | (Some fromId, Some toId) when fromId = toId -> List.singleton sameAccountError
    | _ -> List.empty
