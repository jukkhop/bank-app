namespace Bank

open Bank.Utils
open System

module StringUtils =

  let mkString50 (str: string) =
    match str with
    | x when x.Length > 0 && x.Length <= 50 -> Some(String50 x)
    | _ -> None

  let mkString50OrFail str =
    mkString50 str |> getOrFail (sprintf "Invalid string length: %i" str.Length)

  let camelify (str: string) =
    match Seq.toList str with
    | head :: tail -> (Char.ToLower head :: tail) |> Array.ofList |> String.Concat
    | _ -> str
