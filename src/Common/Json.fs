namespace Bank

open FSharp.Json

module Json =

  let private config =
    JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)

  let serialize (obj: obj) : string =
    Json.serializeEx config obj

  let deserialize<'a> (json: string) : 'a =
    Json.deserializeEx<'a> config json
