namespace Bank

open Amazon.Lambda.APIGatewayEvents

module HttpUtils =

  let headers = Map [
    "Content-Type", "application/json"
  ]

  let mkSuccessResponse (results: 'A list) =
    APIGatewayProxyResponse(
      StatusCode = 200,
      Body = Json.serialize { Results = results },
      Headers = headers
    )

  let mkGenericErrorResponse (status: int) (message: string) =
    APIGatewayProxyResponse(
      StatusCode = status,
      Body = Json.serialize { Message = message },
      Headers = headers
    )

  let mkValidationErrorResponse (validationErrors: ValidationError list) =
    APIGatewayProxyResponse(
      StatusCode = 400,
      Body = Json.serialize { ValidationErrors = validationErrors },
      Headers = headers
    )
