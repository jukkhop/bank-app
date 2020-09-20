namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.HttpUtils

module GetAccountOwners =

  let handler (_: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    match AccountOwnerDb.getAll() with
    | Ok owners -> successResponse { Owners = owners }
    | Error ex -> genericErrorResponse HttpStatus.InternalError ex.Message

