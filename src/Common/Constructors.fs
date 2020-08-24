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
    | x when x.Length > 0 && x.Length <= 50 -> Some (String50 x)
    | _ -> None

  let mkString50OrFail str =
    mkString50 str |> getOrFail <| sprintf "Invalid string length: %i" str.Length

  let isValidAccountNumber (str: string) =
    let removeWhiteSpace = String.filter (Char.IsWhiteSpace >> not)

    let checkLength (str: string) =
      if (str.Length = 18) then Some str else None

    let moveFourCharsToEnd =
      Seq.toList >> List.splitAt 4 >> toList >> List.rev >> List.concat

    let letterToDigit (ch: char) =
      if Char.IsLetter ch
        then ch |> (Char.ToUpper >> int >> (flip (-) 55) >> string)
        else ch |> string

    let digitize = List.map letterToDigit >> List.reduce (+) >> bigint.Parse
    let checkModulus = (flip (%) 97I) >> (=) 1I

    str |> removeWhiteSpace
        |> checkLength
        |> Option.map (moveFourCharsToEnd >> digitize >> checkModulus)
        |> getOrElse false

  let mkAccountNumberOrFail str =
    if isValidAccountNumber str
      then AccountNumber str
      else failwith <| sprintf "Invalid account number: %s" str