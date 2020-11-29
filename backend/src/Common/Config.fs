namespace Bank

open Bank.Utils
open System

module Config =

  type PostgresConfig =
    { Host: string
      Port: int
      User: string
      Password: string
      Database: string }

  type Config =
    { Postgres: PostgresConfig }

  let defaultConfig: Config = {
    Postgres = {
      Host = "localhost"
      Port = 5434
      User = "bank_user"
      Password = "bank_password"
      Database = "bank_db"
    }
  }

  let private getEnvVar (name: string) : string option =
    match Environment.GetEnvironmentVariable name with
    | null -> None
    | x when String.IsNullOrWhiteSpace(x) -> None
    | value -> Some value

  let private postgresConfig: PostgresConfig = {
    Host = getEnvVar "POSTGRES_HOST" |> getOrElse defaultConfig.Postgres.Host
    Port = getEnvVar "POSTGRES_PORT" |> Option.map int |> getOrElse defaultConfig.Postgres.Port
    User = getEnvVar "POSTGRES_USER" |> getOrElse defaultConfig.Postgres.User
    Password = getEnvVar "POSTGRES_PASSWORD" |> getOrElse defaultConfig.Postgres.Password
    Database = getEnvVar "POSTGRES_DATABASE" |> getOrElse defaultConfig.Postgres.Database
  }

  let get () : Config =
    try { Postgres = postgresConfig }
    with | ex -> raise ex
