namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Amazon.Lambda.Serialization.Json
open Bank.HttpUtils

[<assembly:LambdaSerializer(typeof<JsonSerializer>)>]
do ()

module GetAccountOwners =

  let handler(_: APIGatewayProxyRequest, _: ILambdaContext): APIGatewayProxyResponse =
    match AccountOwnerDb.getAll with
    | Ok owners -> mkResponse 200 owners
    | Error ex -> mkErrorResponse 500 ex.Message
