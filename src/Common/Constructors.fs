namespace Bank

open System

module Constructors =

  let mkNationality (nationality: string) =
    match nationality with
    | "Austria" -> Austria
    | "Denmark" -> Denmark
    | "Netherlands" -> Netherlands
    | "Sweden" -> Sweden
    | value -> failwith <| String.Format("Invalid value for nationality: {0}", value)

  let mkDateTime (date: NpgsqlTypes.NpgsqlDate) =
    DateTime(date.Year, date.Month, date.Day)

  let mkString50 (str: string) =
    match str with
    | x when x.Length > 0 && x.Length <= 50 -> String50 x
    | x -> failwith <| String.Format("Invalid string length: {0}", x.Length)
