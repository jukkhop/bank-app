namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Bank.HttpUtils

module GetBankAccounts =

  let handler(_: APIGatewayProxyRequest, _: ILambdaContext): APIGatewayProxyResponse =
    match BankAccountDb.getAll with
    | Ok accounts -> mkResponse 200 accounts
    | Error ex -> mkErrorResponse 500 ex.Message
