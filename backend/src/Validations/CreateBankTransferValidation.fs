namespace Bank

open Bank.BankAccountDb
open Bank.Validation
open Bank.Validators

module CreateBankTransferValidation =

  let private schema = Map<string, Validator list> [
    "FromAccountId", [ requiredValidator; accountIdExistsValidator accountIdExists ]
    "ToAccountId", [ requiredValidator; accountIdExistsValidator accountIdExists ]
    "AmountEurCents", [ requiredValidator; rangeValidator(1L, 100000000L) ]
  ]

  let private convert (data: CreateBankTransferDto) : ValidCreateBankTransferDto =
    { FromAccountId = data.FromAccountId.Value |> AccountId
      ToAccountId = data.ToAccountId.Value |> AccountId
      AmountEurCents = data.AmountEurCents.Value |> TransferAmount }

  let validate (data: CreateBankTransferDto) : Result<ValidCreateBankTransferDto, ValidationError list> =
    validate data schema |> Result.map convert
