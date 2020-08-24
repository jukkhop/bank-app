namespace Bank

open Npgsql.FSharp

module Database =

  let connectionString =
    Sql.host "localhost"
    |> Sql.port 5434
    |> Sql.username "bank_user"
    |> Sql.password "bank_password"
    |> Sql.database "bank_db"
    |> Sql.formatConnectionString

  let query (sql: string) (readFn: RowReader -> 'A) =
    connectionString
    |> Sql.connect
    |> Sql.query sql
    |> Sql.execute readFn
