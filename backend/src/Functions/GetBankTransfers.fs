namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.BankTransferDb
open Bank.Context
open Bank.HttpUtils

module GetBankTransfers =

  let impl (_: APIGatewayProxyRequest) (db: IBankTransferDb) : APIGatewayProxyResponse =
    match db.GetAll() with
    | Ok tranfers -> successResponse { Transfers = tranfers }
    | Error ex -> genericErrorResponse HttpStatus.InternalError ex.Message

  let handler (request: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    let context = Context (Config.get())
    impl request context.TransferDb
