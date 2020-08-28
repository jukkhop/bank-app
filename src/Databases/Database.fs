namespace Bank

open Npgsql
open Npgsql.FSharp

module Database =

  type SqlProps = Sql.SqlProps
  type Transaction = NpgsqlTransaction

  let private connectionString: string =
    Sql.host "localhost"
    |> Sql.port 5434
    |> Sql.username "bank_user"
    |> Sql.password "bank_password"
    |> Sql.database "bank_db"
    |> Sql.formatConnectionString

  let private connect: SqlProps =
    Sql.connect connectionString

  let query (sql: string) (readFn: RowReader -> 'a) : Result<'a list, exn> =
    connect
    |> Sql.query sql
    |> Sql.execute readFn

  let row (sql: string) (readFn: RowReader -> 'a) : Result<'a, exn> =
    connect
    |> Sql.query sql
    |> Sql.executeRow readFn

  let nonQuery (sql: string) : Result<int, exn> =
    connect
    |> Sql.query sql
    |> Sql.executeNonQuery

  let inTransaction (fn: Transaction -> 'a) : 'a =
    use connection = new NpgsqlConnection(connectionString)
    connection.Open()
    use transaction = connection.BeginTransaction()
    let result = fn transaction
    transaction.Commit()
    result

  let queryTx (tx: Transaction) (sql: string) (readFn: RowReader -> 'a) : Result<'a list, exn> =
    tx.Connection
    |> Sql.existingConnection
    |> Sql.query sql
    |> Sql.execute readFn

  let rowTx (tx: Transaction) (sql: string) (readFn: RowReader -> 'a) : Result<'a, exn> =
    tx.Connection
    |> Sql.existingConnection
    |> Sql.query sql
    |> Sql.executeRow readFn

  let nonQueryTx (tx: Transaction) (sql: string) : Result<int, exn> =
    tx.Connection
    |> Sql.existingConnection
    |> Sql.query sql
    |> Sql.executeNonQuery
