namespace Bank

open Bank.Operators
open Bank.StringUtils

module Validators =

  let private mkError (field: string) (message: string) : ValidationError =
    { Field = field |> camelify; Message = message }

  let requiredValidator (field: string) (value: obj option) : ValidationError option =
    match value with
    | Some _ -> None
    | None -> Some <| mkError field "Value is required"

  let rangeValidator (min: int64, max: int64) (field: string) (value: obj option) : ValidationError option =
    match Option.get value |> unbox<int64> with
    | x when x >=< (min, max) -> None
    | _ -> sprintf "Value is not within range, min: %i, max: %i" min max |> mkError field |> Some

  let accountIdExistsValidator (exists: int64 -> Result<bool, exn>) (field: string) (value: obj option) : ValidationError option =
    let id = Option.get value |> unbox<int64>
    match exists id with
    | Ok true -> None
    | Ok false -> sprintf "Bank account with account ID %i not found" id |> mkError field |> Some
    | Error ex -> failwithf "Database error while checking account ID: %s" ex.Message
