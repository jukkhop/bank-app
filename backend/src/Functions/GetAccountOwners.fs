namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.AccountOwnerDb
open Bank.Context
open Bank.HttpUtils

module GetAccountOwners =

  let impl (_: APIGatewayProxyRequest) (db: IAccountOwnerDb) : APIGatewayProxyResponse =
    match db.GetAll() with
    | Ok owners -> successResponse { Owners = owners }
    | Error ex -> genericErrorResponse HttpStatus.InternalError ex.Message

  let handler (request: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    let context = Context (Config.get())
    impl request context.OwnerDb
