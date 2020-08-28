namespace Bank

open Bank.Utils
open Bank.Validators
open Microsoft.FSharp.Reflection

module CreateBankTransferValidation =

  let private validationSchema = Map<string, Validator list> [
    "FromAccountId", [ requiredValidator ]
    "ToAccountId", [ requiredValidator ]
    "AmountEurCents", [ requiredValidator ]
  ]

  let validate (data: CreateBankTransferDto): Result<Unit, ValidationError list> =

    let values =
      data.GetType() |> FSharpType.GetRecordFields
                     |> Seq.map (fun prop -> prop.Name, prop.GetValue(data) |> nullableToOption)
                     |> Map.ofSeq

    let validateValue field value validator = validator field value

    let collectErrors (field, validators) =
      validators |> List.collect (validateValue field <| values.Item field)

    match validationSchema |> Map.toList |> List.collect collectErrors with
    | errors when errors.IsEmpty -> Ok ()
    | errors -> Error errors
