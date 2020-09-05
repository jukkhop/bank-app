namespace Bank

open Bank.Utils
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

  let private toSqlType (v: obj) =
    match v with
    | :? string as v -> Sql.string v
    | :? int64 as v -> Sql.int64 v
    | _ -> failwithf "Unable to match type: %s" <| v.GetType().ToString()

  let private mkParams (parms: list<string * obj>) =
    parms |> List.map (fun (k, v) -> k, toSqlType v)

  let private mkQuery (sql: string) (parms: list<string * obj>) : SqlProps =
    connectionString
    |> Sql.connect
    |> Sql.query sql
    |> Sql.parameters (mkParams parms)

  let private mkQueryTx (tx: Transaction) (sql: string) (parms: list<string * obj>) : SqlProps =
    tx.Connection
    |> Sql.existingConnection
    |> Sql.query sql
    |> Sql.parameters (mkParams parms)

  let query (sql: string) (parms: list<string * obj>) (readFn: RowReader -> 'a) : Result<'a list, exn> =
    mkQuery sql parms |> Sql.execute readFn

  let row (sql: string) (parms: list<string * obj>) (readFn: RowReader -> 'a) : Result<'a, exn> =
    mkQuery sql parms |> Sql.executeRow readFn

  let nonQuery (sql: string) (parms: list<string * obj>) : Result<unit, exn> =
    mkQuery sql parms |> Sql.executeNonQuery |> unitize

  let inTransaction (fn: Transaction -> 'a) : 'a =
    use connection = new NpgsqlConnection(connectionString)
    connection.Open()
    use transaction = connection.BeginTransaction()
    let result = fn transaction
    transaction.Commit()
    result

  let queryTx (tx: Transaction) (sql: string) (parms: list<string * obj>) (readFn: RowReader -> 'a) : Result<'a list, exn> =
    mkQueryTx tx sql parms |> Sql.execute readFn

  let rowTx (tx: Transaction) (sql: string) (parms: list<string * obj>) (readFn: RowReader -> 'a) : Result<'a, exn> =
    mkQueryTx tx sql parms |> Sql.executeRow readFn

  let nonQueryTx (tx: Transaction) (sql: string) (parms: list<string * obj>) : Result<unit, exn> =
    mkQueryTx tx sql parms |> Sql.executeNonQuery |> unitize
