namespace Bank

open Bank.Utils
open Npgsql
open Npgsql.FSharp

module Database =

  type Transaction = NpgsqlTransaction

  type private SqlProps = Sql.SqlProps
  type private Params = list<string * obj>
  type private Read<'a> = RowReader -> 'a

  let private connectionString () : string =
    let config = Config.getUnsafe.Postgres
    Sql.host config.Host
    |> Sql.port config.Port
    |> Sql.username config.User
    |> Sql.password config.Password
    |> Sql.database config.Database
    |> Sql.formatConnectionString

  let private toSqlType (v: obj) =
    match v with
    | :? string as v -> Sql.string v
    | :? int64 as v -> Sql.int64 v
    | _ -> failwithf "Unable to match type: %s" <| v.GetType().ToString()

  let private mkParams (parms: Params) =
    parms |> List.map (fun (k, v) -> k, toSqlType v)

  let private mkQuery (sql: string) (parms: Params) : SqlProps =
    connectionString()
    |> Sql.connect
    |> Sql.query sql
    |> Sql.parameters (mkParams parms)

  let private mkQueryTx (tx: Transaction) (sql: string) (parms: Params) : SqlProps =
    tx.Connection
    |> Sql.existingConnection
    |> Sql.query sql
    |> Sql.parameters (mkParams parms)

  let query (sql: string) (parms: Params) (read: Read<'a>) : Result<'a list, exn> =
    mkQuery sql parms |> Sql.execute read

  let row (sql: string) (parms: Params) (read: Read<'a>) : Result<'a, exn> =
    mkQuery sql parms |> Sql.executeRow read

  let nonQuery (sql: string) (parms: Params) : Result<unit, exn> =
    mkQuery sql parms |> Sql.executeNonQuery |> unitize

  let inTransaction (fn: Transaction -> Result<'a, 'b>) : Result<Result<'a, 'b>, exn> =
    try
      use connection = new NpgsqlConnection(connectionString())
      do connection.Open()
      use transaction = connection.BeginTransaction()
      let result = fn transaction
      match result with
        | Ok _ -> do transaction.Commit()
        | Error _ -> do transaction.Rollback()
      Ok result
    with
      | ex -> Error ex

  let queryTx (tx: Transaction) (sql: string) (parms: Params) (read: Read<'a>) : Result<'a list, exn> =
    mkQueryTx tx sql parms |> Sql.execute read

  let rowTx (tx: Transaction) (sql: string) (parms: Params) (read: Read<'a>) : Result<'a, exn> =
    mkQueryTx tx sql parms |> Sql.executeRow read

  let nonQueryTx (tx: Transaction) (sql: string) (parms: Params) : Result<unit, exn> =
    mkQueryTx tx sql parms |> Sql.executeNonQuery |> unitize
