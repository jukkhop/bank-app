namespace Bank

open Bank.Patterns
open Bank.Utils
open Microsoft.FSharp.Reflection

module Validation =

  let validate (data: 'a) (schema: Map<string, Validator list>) : Result<Unit, ValidationError list> =

    let values =
      data.GetType() |> FSharpType.GetRecordFields
                     |> Seq.map (fun prop -> prop.Name, prop.GetValue(data) |> nullableToOption)
                     |> Map.ofSeq

    let validateValue field value validator = validator field value

    let collectErrors (field, validators) =
      validators |> List.collect (validateValue field <| values.Item field)

    match schema |> Map.toList |> List.collect collectErrors with
    | Empty -> Ok ()
    | NotEmpty errors -> Error errors
