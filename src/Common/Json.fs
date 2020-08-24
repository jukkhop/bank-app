namespace Bank

open FSharp.Json

module Json =

  let private config =
    JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)

  let serialize obj =
    Json.serializeEx config obj
    
  let deserialize json =
    Json.deserializeEx config json
