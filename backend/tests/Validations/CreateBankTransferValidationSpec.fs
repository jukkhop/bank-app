module Tests

open Bank
open Bank.BankAccountDb
open Bank.TestData
open Foq
open FsUnit.Xunit
open Xunit

module CreateBankTransferValidationSpec =

  let validData: CreateBankTransferDto =
    { FromAccountId = Some accountId1Int
      ToAccountId = Some accountId2Int
      AmountEurCents = Some transfer1AmountInt }

  let validatedData: ValidCreateBankTransferDto =
    { FromAccountId = accountId1
      ToAccountId = accountId2
      AmountEurCents = transfer1Amount }

  let emptyData: CreateBankTransferDto =
    { FromAccountId = None
      ToAccountId = None
      AmountEurCents = None }

  let accountDbMock = Mock<IBankAccountDb>.With(fun mock ->
    <@
      mock.AccountIdExists accountId1Int --> Ok true
      mock.AccountIdExists accountId2Int --> Ok true
      mock.AccountIdExists (any()) --> Ok false
    @>
  )

  let validate = CreateBankTransferValidation.validate accountDbMock
  let mkError (field: string) (message: string) : ValidationError = { Field = field; Message = message }

  let amountMissingError = mkError "amountEurCents" "Value is required"
  let fromAccountMissingError =  mkError "fromAccountId" "Value is required"
  let toAccountMissingError = mkError "toAccountId" "Value is required"
  let invalidAccountIdError field id = mkError field (sprintf "Bank account with account ID %i not found" id)

  let sameAccountsError =
    mkError "(fromAccountId,toAccountId)" "Source and destination accounts cannot be the same account"

  let invalidAmountError min max =
    mkError "amountEurCents" (sprintf "Value is not within range, min: %i, max: %i" min max)

  let shouldPass (data: CreateBankTransferDto) =
    match validate data with
    | Ok x -> x |> should be ofExactType<ValidCreateBankTransferDto>
    | Error _ -> failwith "got an Error, expected Ok"

  let shouldFailWith (expected: ValidationError list) (data: CreateBankTransferDto) : unit =
    match validate data with
    | Ok _ -> failwith "got an Ok, expected Error"
    | Error actual -> List.map (fun err -> actual |> should contain err) expected |> ignore

  [<Fact>]
  let ``accept valid data`` () =
    validData |> shouldPass

  [<Fact>]
  let ``reject empty data`` () =
    emptyData |> shouldFailWith [amountMissingError; fromAccountMissingError; toAccountMissingError]

  [<Fact>]
  let ``reject if amount is missing`` () =
    { validData with AmountEurCents = None } |> shouldFailWith [amountMissingError]

  [<Fact>]
  let ``reject if from account ID is missing`` () =
    { validData with FromAccountId = None } |> shouldFailWith [fromAccountMissingError]

  [<Fact>]
  let ``reject if to account ID is missing`` () =
    { validData with ToAccountId = None } |> shouldFailWith [toAccountMissingError]

  [<Fact>]
  let ``reject if the from account ID does not exist`` () =
    { validData with FromAccountId = Some 123L } |> shouldFailWith [invalidAccountIdError "fromAccountId" 123L]

  [<Fact>]
  let ``reject if the to account ID does not exist`` () =
    { validData with ToAccountId = Some 123L } |> shouldFailWith [invalidAccountIdError "toAccountId" 123L]

  [<Fact>]
  let ``reject if the from and to account IDs are the same`` () =
    { validData with FromAccountId = Some 123L; ToAccountId = Some 123L } |> shouldFailWith [sameAccountsError]

  [<Fact>]
  let ``reject if amount is less than the minimum allowed`` () =
    { validData with AmountEurCents = Some 0L; } |> shouldFailWith [invalidAmountError 1L 100000000L]

  [<Fact>]
  let ``reject if amount is more than the maximum allowed`` () =
    { validData with AmountEurCents = Some 100000001L; } |> shouldFailWith [invalidAmountError 1L 100000000L]
