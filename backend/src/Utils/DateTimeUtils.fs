namespace Bank

open System

module DateTimeUtils =

  let mkDateTime (date: NpgsqlTypes.NpgsqlDate) =
    DateTime(date.Year, date.Month, date.Day)
