namespace Bank

open Bank.Validation
open Bank.Validators

module CreateBankTransferValidation =

  let private schema = Map<string, Validator list> [
    "FromAccountId", [ requiredValidator ]
    "ToAccountId", [ requiredValidator ]
    "AmountEurCents", [ requiredValidator ]
  ]

  let validate (data: CreateBankTransferDto) : Result<Unit, ValidationError list> =
    validate data schema
