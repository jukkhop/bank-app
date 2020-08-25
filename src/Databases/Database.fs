namespace Bank

open Npgsql.FSharp

module Database =

  let connectionString: string =
    Sql.host "localhost"
    |> Sql.port 5434
    |> Sql.username "bank_user"
    |> Sql.password "bank_password"
    |> Sql.database "bank_db"
    |> Sql.formatConnectionString

  let query (sql: string) (readFn: RowReader -> 'A) : Result<'A list, exn> =
    connectionString
    |> Sql.connect
    |> Sql.query sql
    |> Sql.execute readFn
