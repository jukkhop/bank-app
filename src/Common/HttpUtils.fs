namespace Bank

open Amazon.Lambda.APIGatewayEvents

module HttpUtils =

  let headers = Map [
    "Content-Type", "application/json"
  ]

  let mkResponse (status: int) (results: 'A list) =
    APIGatewayProxyResponse(
      StatusCode = status,
      Body = Json.serialize { Results = results },
      Headers = headers
    )

  let mkErrorResponse (status: int) (message: string) =
    APIGatewayProxyResponse(
      StatusCode = status,
      Body = Json.serialize { Message = message },
      Headers = headers
    )
