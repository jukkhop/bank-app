namespace Bank

open Bank.Utils
open System

module Constructors =

  let mkDateTime (date: NpgsqlTypes.NpgsqlDate) =
    DateTime(date.Year, date.Month, date.Day)

  let mkNationality str =
    match str with
    | "Austria" -> Some Austria
    | "Denmark" -> Some Denmark
    | "Netherlands" -> Some Netherlands
    | "Sweden" -> Some Sweden
    | _ -> None

  let mkNationalityOrFail str =
    mkNationality str |> getOrFail <| sprintf "Invalid value for nationality: %s" str

  let mkString50 (str: string) =
    match str with
    | x when x.Length > 0 && x.Length <= 50 -> Some(String50 x)
    | _ -> None

  let mkString50OrFail str =
    mkString50 str |> getOrFail <| sprintf "Invalid string length: %i" str.Length

  let mkAccountNumberOrFail str =
    if AccountNumberUtils.isValid str
      then AccountNumber str
      else failwith <| sprintf "Invalid account number: %s" str

  let mkTransferResult str =
    match str with
    | "Success" -> Some Success
    | "InsufficientFunds" -> Some InsufficientFunds
    | "NetworkError" -> Some NetworkError
    | "OtherError" -> Some OtherError
    | _ -> None

  let mkTransferResultOrFail str =
    mkTransferResult str |> getOrFail <| sprintf "Invalid value for transfer result: %s" str
