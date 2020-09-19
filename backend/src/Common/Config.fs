namespace Bank

open Bank.Utils
open System

module Config =

  type PostgresConfig = {
    Host: string
    Port: int
    User: string
    Password: string
    Database: string
  }

  type Config = {
    Postgres: PostgresConfig
  }

  let private getEnvVar (name: string) : string option =
    match Environment.GetEnvironmentVariable name with
    | null -> None
    | x when String.IsNullOrWhiteSpace(x) -> None
    | value -> Some value

  let private getEnvVarOrFail (name: string) : string =
    getEnvVar name |> getOrFail (sprintf "Environment variable %s not set" name)

  let postgresConfig = {
    Host = getEnvVarOrFail "POSTGRES_HOST"
    Port = getEnvVarOrFail "POSTGRES_PORT" |> int
    User = getEnvVarOrFail "POSTGRES_USER"
    Password = getEnvVarOrFail "POSTGRES_PASSWORD"
    Database = getEnvVarOrFail "POSTGRES_DATABASE"
  }

  let getUnsafe =
    try
      { Postgres = postgresConfig }
    with
    | ex -> raise ex
