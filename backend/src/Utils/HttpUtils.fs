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

  let deserialize<'a> (json: string) : Result<'a, ValidationError list> =
    Json.deserialize<'a> json
    |> Result.mapError (fun ex -> [{ Field = String.empty
                                     Message = sprintf "Parse error: %s" ex.Message }])

  let successResponse (body: 'a) =
    mkResponse HttpStatus.Ok <| Json.serialize body

  let errorResponse (status: HttpStatus) (error: ErrorBody) =
    mkResponse status <| Json.serialize error

  let genericErrorResponse (status: HttpStatus) (message: string) =
    mkResponse status <| Json.serialize { Message = message }

  let parseErrorResponse (ex: exn) =
    mkResponse HttpStatus.BadRequest <| Json.serialize { Message = ex.Message }

  let validationErrorResponse (validationErrors: ValidationError list) =
    mkResponse HttpStatus.BadRequest <| Json.serialize { ValidationErrors = validationErrors }
