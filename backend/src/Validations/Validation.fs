namespace Bank

open Bank.Utils
open Microsoft.FSharp.Reflection

module Validation =

  let collectErrors (data: 'a) (schema: Map<string, Validator list>) : ValidationError list =

    let values: Map<string, obj option> =
      data.GetType() |> FSharpType.GetRecordFields
                     |> Seq.map (fun prop -> prop.Name, prop.GetValue(data) |> nullableToOption)
                     |> Map.ofSeq

    let validateValue (field: string) (value: obj option) (validator: Validator) : ValidationError option =
      validator field value

    let collectFieldErrors (field: string, validators: Validator list) : ValidationError list =
      validators
      |> List.map (validateValue field (values.Item field))
      |> List.choose id

    schema |> Map.toList |> List.collect collectFieldErrors

  let validate (data: 'a) (schema: Map<string, Validator list>) : Result<'a, ValidationError list> =
    match collectErrors data schema with
    | [] -> Ok data
    | errors -> Error errors
