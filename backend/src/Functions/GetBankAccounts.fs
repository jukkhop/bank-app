namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.BankAccountDb
open Bank.HttpUtils

module GetBankAccounts =

  let impl (_: APIGatewayProxyRequest) (db: IBankAccountDb) : APIGatewayProxyResponse =
    match db.GetAll() with
    | Ok accounts -> successResponse { Accounts = accounts }
    | Error ex -> genericErrorResponse HttpStatus.InternalError ex.Message

  let handler (request: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    let db = BankAccountDb()
    impl request (db :> IBankAccountDb)
