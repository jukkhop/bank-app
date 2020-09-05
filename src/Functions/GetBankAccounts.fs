namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.HttpUtils

module GetBankAccounts =

  let handler(_: APIGatewayProxyRequest): APIGatewayProxyResponse =
    match BankAccountDb.getAll() with
    | Ok accounts -> mkSuccessResponse accounts
    | Error ex -> mkGenericErrorResponse 500 ex.Message
