namespace Bank

open Bank.Config
open Bank.Utils
open Npgsql
open Npgsql.FSharp
open System
open System.Data
open System.Data.Common

module Database =

  type Transaction = DbTransaction
  type private SqlProps = Sql.SqlProps
  type private Params = list<string * obj>
  type private Read<'a> = RowReader -> 'a

  let private isolationLevel = IsolationLevel.Serializable

  type DbHelpers =
    static member ToSqlType (v: obj) =
      match v with
      | :? string as v -> Sql.string v
      | :? int64 as v -> Sql.int64 v
      | :? DateTime as v -> Sql.timestamptz v
      | _ -> failwithf "Unable to match type: %s" <| v.GetType().ToString()

    static member MkParams (parms: Params) =
      parms |> List.map (fun (k, v) -> k, DbHelpers.ToSqlType v)

  type IDatabase =
    abstract Query: string -> Params -> Read<'a> -> Result<'a list, exn>
    abstract Row: string -> Params -> Read<'a> -> Result<'a, exn>
    abstract NonQuery: string -> Params -> Result<unit, exn>
    abstract InTransaction: (Transaction -> Result<'a, 'b>) -> Result<Result<'a, 'b>, exn>
    abstract QueryTx: Transaction -> string -> Params -> Read<'a> -> Result<'a list, exn>
    abstract RowTx: Transaction -> string -> Params -> Read<'a> -> Result<'a, exn>
    abstract NonQueryTx: Transaction -> string -> Params -> Result<unit, exn>

  type Database (config: PostgresConfig) as this =
    member __.ConnectionString () : string =
      Sql.host config.Host
      |> Sql.port config.Port
      |> Sql.username config.User
      |> Sql.password config.Password
      |> Sql.database config.Database
      |> Sql.formatConnectionString

    member __.MkQuery (sql: string) (parms: Params) : SqlProps =
      this.ConnectionString()
      |> Sql.connect
      |> Sql.query sql
      |> Sql.parameters (DbHelpers.MkParams parms)

    member __.MkQueryTx (tx: Transaction) (sql: string) (parms: Params) : SqlProps =
      (tx :?> NpgsqlTransaction).Connection
      |> Sql.existingConnection
      |> Sql.query sql
      |> Sql.parameters (DbHelpers.MkParams parms)

    interface IDatabase with
      member __.Query (sql: string) (parms: Params) (read: Read<'a>) : Result<'a list, exn> =
        this.MkQuery sql parms |> Sql.execute read

      member __.Row (sql: string) (parms: Params) (read: Read<'a>) : Result<'a, exn> =
        this.MkQuery sql parms |> Sql.executeRow read

      member __.NonQuery (sql: string) (parms: Params) : Result<unit, exn> =
        this.MkQuery sql parms |> Sql.executeNonQuery |> unitize

      member __.InTransaction (fn: Transaction -> Result<'a, 'b>) : Result<Result<'a, 'b>, exn> =
        try
          use connection = new NpgsqlConnection(this.ConnectionString())
          do connection.Open()
          use transaction = connection.BeginTransaction(isolationLevel)
          let result = fn transaction
          match result with
            | Ok _ -> do transaction.Commit()
            | Error _ -> do transaction.Rollback()
          Ok result
        with
          | ex -> Error ex

      member __.QueryTx (tx: Transaction) (sql: string) (parms: Params) (read: Read<'a>) : Result<'a list, exn> =
        this.MkQueryTx tx sql parms |> Sql.execute read

      member __.RowTx (tx: Transaction) (sql: string) (parms: Params) (read: Read<'a>) : Result<'a, exn> =
        this.MkQueryTx tx sql parms |> Sql.executeRow read

      member __.NonQueryTx (tx: Transaction) (sql: string) (parms: Params) : Result<unit, exn> =
        this.MkQueryTx tx sql parms |> Sql.executeNonQuery |> unitize
