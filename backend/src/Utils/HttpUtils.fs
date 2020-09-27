namespace Bank

open Amazon.Lambda.APIGatewayEvents
open FSharp.Core.LanguagePrimitives

module HttpUtils =

  let private headers = Map [
    "Access-Control-Allow-Credentials", "true"
    "Access-Control-Allow-Origin", "*"
    "Content-Type", "application/json"
  ]

  let private response (status: HttpStatus) (body: string) =
    APIGatewayProxyResponse(
      StatusCode = EnumToValue status,
      Body = body,
      Headers = headers
    )

  let successResponse (body: 'a) =
    response HttpStatus.Ok <| Json.serialize body

  let errorResponse (status: HttpStatus) (error: ErrorBody) =
    response status <| Json.serialize error

  let genericErrorResponse (status: HttpStatus) (message: string) =
    response status <| Json.serialize { Message = message }

  let parseErrorResponse (ex: exn) =
    response HttpStatus.BadRequest <| Json.serialize { Message = ex.Message }

  let validationErrorResponse (validationErrors: ValidationError list) =
    response HttpStatus.BadRequest <| Json.serialize { ValidationErrors = validationErrors }
