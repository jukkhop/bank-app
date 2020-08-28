namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Bank.HttpUtils

module CreateBankTransfer =

  let handler(req: APIGatewayProxyRequest, _: ILambdaContext): APIGatewayProxyResponse =

    let validationSuccess (data: CreateBankTransferDto) =
      match BankTransferDb.makeTransfer
        (AccountId data.FromAccountId.Value)
        (AccountId data.ToAccountId.Value)
        (TransferAmount data.AmountEurCents.Value) with
      | Ok transfer -> mkSuccessResponse [transfer]
      | Error ex -> mkGenericErrorResponse 500 ex.Message

    let data = Json.deserialize<CreateBankTransferDto> req.Body

    match CreateBankTransferValidation.validate data with
    | Ok () -> validationSuccess data
    | Error errors -> mkValidationErrorResponse errors
