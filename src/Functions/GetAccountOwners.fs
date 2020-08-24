namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Amazon.Lambda.Serialization.Json
open Bank.Headers

[<assembly:LambdaSerializer(typeof<JsonSerializer>)>]
do ()

module GetAccountOwners =

  let handler(_: APIGatewayProxyRequest, _: ILambdaContext): APIGatewayProxyResponse =
    let (statusCode, body) =
      match AccountOwnerDb.getAll with
      | Ok owners -> 200, Json.serialize { Results = owners }
      | Error ex -> 500, Json.serialize { Message = ex.Message }

    APIGatewayProxyResponse(
      StatusCode = statusCode,
      Body = body,
      Headers = headers
    )
