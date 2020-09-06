namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.HttpUtils

module GetBankAccounts =

  let handler (_: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    match BankAccountDb.getAll() with
    | Ok accounts -> mkSuccessResponse { Accounts = accounts }
    | Error ex -> mkGenericErrorResponse HttpStatus.InternalError ex.Message
