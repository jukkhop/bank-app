namespace Bank

open FSharp.Json

module Json =

  let private config =
    JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase, unformatted = true)

  let serialize (obj: obj) : string =
    Json.serializeEx config obj

  let deserialize<'a> (json: string) : Result<'a, exn> =
    try
      json |> Json.deserializeEx<'a> config |> Ok
    with
    | ex -> Error ex
