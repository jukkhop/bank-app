namespace Bank

open Bank.Utils
open System

module AccountNumberUtils =

  let isValid (cand: string) =
    let removeWhiteSpace = String.filter (Char.IsWhiteSpace >> not)

    let checkLength (str: string) =
      if str.Length = 18 then Some str else None

    let moveFourCharsToEnd =
      Seq.toList >> List.splitAt 4 >> toList >> List.rev >> List.concat

    let letterToDigit (ch: char) =
      if Char.IsLetter ch
        then ch |> (Char.ToUpper >> int >> (flip (-) 55) >> string)
        else ch |> string

    let digitize = List.map letterToDigit >> List.reduce (+) >> bigint.Parse
    let checkModulus = (flip (%) 97I) >> (=) 1I

    cand |> removeWhiteSpace
         |> checkLength
         |> Option.map (moveFourCharsToEnd >> digitize >> checkModulus)
         |> getOrElse false

  let mkAccountNumberOrFail str =
    if isValid str
      then AccountNumber str
      else failwith <| sprintf "Invalid account number: %s" str
