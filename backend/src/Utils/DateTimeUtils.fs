namespace Bank

open System

module DateTimeUtils =

  let mkDateTime (date: NpgsqlTypes.NpgsqlDate) =
    date.AddDays(1) |> fun dt -> DateTime (dt.Year, dt.Month, dt.Day)

