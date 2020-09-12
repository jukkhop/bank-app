namespace Bank

open Bank.Validation
open Bank.Validators

module CreateBankTransferValidation =

  let private schema = Map<string, Validator list> [
    "FromAccountId", [ requiredValidator ]
    "ToAccountId", [ requiredValidator ]
    "AmountEurCents", [ requiredValidator ]
  ]

  let private convert (data: CreateBankTransferDto) : ValidCreateBankTransferDto =
    { FromAccountId = data.FromAccountId.Value |> AccountId
      ToAccountId = data.ToAccountId.Value |> AccountId
      AmountEurCents = data.AmountEurCents.Value |> TransferAmount }

  let validate (data: CreateBankTransferDto) : Result<ValidCreateBankTransferDto, ValidationError list> =
    validate data schema |> Result.map convert
