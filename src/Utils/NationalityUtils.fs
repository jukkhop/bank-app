namespace Bank

open Bank.Utils

module NationalityUtils =

  let mkNationality = function
    | "Austria" -> Some Austria
    | "Denmark" -> Some Denmark
    | "Netherlands" -> Some Netherlands
    | "Sweden" -> Some Sweden
    | _ -> None

  let mkNationalityOrFail str =
    mkNationality str |> getOrFail <| sprintf "Invalid value for nationality: %s" str
