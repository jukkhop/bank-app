namespace Bank

open Bank.BankAccountDb
open Bank.Validation
open Bank.Validators

module CreateBankTransferValidation =

  let private schema (db: IBankAccountDb) = Map<string, Validator list> [
    "FromAccountId", [ requiredValidator; accountIdExistsValidator db.AccountIdExists ]
    "ToAccountId", [ requiredValidator; accountIdExistsValidator db.AccountIdExists ]
    "AmountEurCents", [ requiredValidator; rangeValidator(1L, 100000000L) ]
  ]

  let private convert (data: CreateBankTransferDto) : ValidCreateBankTransferDto =
    { FromAccountId = data.FromAccountId.Value |> AccountId
      ToAccountId = data.ToAccountId.Value |> AccountId
      AmountEurCents = data.AmountEurCents.Value |> TransferAmount }

  let validate (db: IBankAccountDb) (data: CreateBankTransferDto) : Result<ValidCreateBankTransferDto, ValidationError list> =
    let validationErrors =
      List.append (collectErrors data (schema db)) (differentAccountsValidator data)

    match validationErrors with
    | [] -> Ok (convert data)
    | errors -> Error errors
