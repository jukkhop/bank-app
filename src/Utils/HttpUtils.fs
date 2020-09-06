namespace Bank

open Amazon.Lambda.APIGatewayEvents
open FSharp.Core.LanguagePrimitives

module HttpUtils =

  let private headers = Map [
    "Content-Type", "application/json"
  ]

  let private mkResponse (status: HttpStatus) (body: string) =
    APIGatewayProxyResponse(
      StatusCode = EnumToValue status,
      Body = body,
      Headers = headers
    )

  let mkSuccessResponse (body: 'a) =
    mkResponse HttpStatus.Ok <| Json.serialize body

  let mkGenericErrorResponse (status: HttpStatus) (message: string) =
    mkResponse status <| Json.serialize { Message = message }

  let mkErrorResponse (status: HttpStatus) (error: ErrorBody) =
    mkResponse status <| Json.serialize error

  let mkValidationErrorResponse (validationErrors: ValidationError list) =
    mkResponse HttpStatus.BadRequest <| Json.serialize { ValidationErrors = validationErrors }
