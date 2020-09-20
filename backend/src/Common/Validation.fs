namespace Bank

open Bank.Patterns
open Bank.Utils
open Microsoft.FSharp.Reflection

module Validation =

  let validate (data: 'a) (schema: Map<string, Validator list>) : Result<'a, ValidationError list> =

    let values: Map<string, obj option> =
      data.GetType() |> FSharpType.GetRecordFields
                     |> Seq.map (fun prop -> prop.Name, prop.GetValue(data) |> nullableToOption)
                     |> Map.ofSeq

    let validateValue (field: string) (value: obj option) (validator: Validator) =
      validator field value

    let collectErrors (field: string, validators: Validator list) =
      validators
      |> List.map (validateValue field <| values.Item field)
      |> List.choose id

    match schema |> Map.toList |> List.collect collectErrors with
    | [] -> Ok data
    | errors -> Error errors
