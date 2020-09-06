namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Amazon.Lambda.Serialization.Json
open Bank.HttpUtils

[<assembly:LambdaSerializer(typeof<JsonSerializer>)>]
do ()

module GetAccountOwners =

  let handler (_: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    match AccountOwnerDb.getAll() with
    | Ok owners -> mkSuccessResponse { Owners = owners }
    | Error ex -> mkGenericErrorResponse HttpStatus.InternalError ex.Message
