namespace Bank

open System

module Constructors =

  let mkNationality str =
    match str with
    | "Austria" -> Some Austria
    | "Denmark" -> Some Denmark
    | "Netherlands" -> Some Netherlands
    | "Sweden" -> Some Sweden
    | _ -> None

  let mkDateTime (date: NpgsqlTypes.NpgsqlDate) =
    DateTime(date.Year, date.Month, date.Day)

  let getOrFail value msg =
    match value with
    | Some x -> x
    | None -> failwith msg

  let mkNationalityOrFail str =
    mkNationality str |> getOrFail <| sprintf "Invalid value for nationality: %s" str

  let mkString50 (str: string) =
    match str with
    | x when x.Length > 0 && x.Length <= 50 -> Some (String50 x)
    | _ -> None

  let mkString50OrFail str =
    mkString50 str |> getOrFail <| sprintf "Invalid string length: %i" str.Length
