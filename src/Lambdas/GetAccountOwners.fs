namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Amazon.Lambda.Serialization.Json
open Bank.Headers
open FSharp.Json

[<assembly:LambdaSerializer(typeof<JsonSerializer>)>]
do ()

type OkBody = {
  Results: AccountOwner list
}

type ErrorBody = {
  Message: string
}

type Body =
  | Ok of OkBody
  | Error of ErrorBody

module GetAccountOwners =

  let handler(_: APIGatewayProxyRequest, _: ILambdaContext): APIGatewayProxyResponse =
    let (statusCode, body) =
      try
        let accountOwners = AccountOwnerDb.getAll
        (200, Ok { Results = accountOwners })
      with
      | ex -> (504, Error { Message = ex.Message })

    APIGatewayProxyResponse(
      StatusCode = statusCode,
      Body = Json.serialize body,
      Headers = headers
    )
