namespace Bank

open Bank.Operators
open System

module Validators =

  let private camelify (str: string) =
    match Seq.toList str with
    | head :: tail -> (Char.ToLower head :: tail) |> Array.ofList |> String.Concat
    | [] -> str

  let private mkError (field: string) (message: string) : ValidationError =
    { Field = camelify field; Message = message }

  let requiredValidator (field: string) (value: obj option) : ValidationError option =
    match value with
    | Some _ -> None
    | None -> Some <| mkError field "Value is required"

  let rangeValidator (min: int64, max: int64) (field: string) (value: obj option) : ValidationError option =
    let value = Option.get value |> unbox<int64>
    match value with
    | x when x >=< (min, max) -> None
    | _ -> sprintf "Value is not within range, min: %i, max: %i" min max |> mkError field |> Some

  let accountIdExistsValidator (exists: int64 -> Result<bool, exn>) (field: string) (value: obj option) : ValidationError option =
    let id = Option.get value |> unbox<int64>
    let mkError = mkError field
    match exists id with
    | Ok true -> None
    | Ok false -> sprintf "Bank account with account ID %i not found" id |> mkError |> Some
    | Error ex -> sprintf "Database error while checking account ID: %s" ex.Message |> mkError |> Some
