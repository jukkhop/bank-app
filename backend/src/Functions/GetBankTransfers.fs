namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.HttpUtils

module GetBankTransfers =

  let handler (_: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    match BankTransferDb.getAll() with
    | Ok tranfers -> successResponse { Transfers = tranfers }
    | Error ex -> genericErrorResponse HttpStatus.InternalError ex.Message
