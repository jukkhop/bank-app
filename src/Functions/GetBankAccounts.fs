namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.HttpUtils

module GetBankAccounts =

  let handler (_: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    match BankAccountDb.getAll() with
    | Ok accounts -> successResponse { Accounts = accounts }
    | Error ex -> genericErrorResponse HttpStatus.InternalError ex.Message
