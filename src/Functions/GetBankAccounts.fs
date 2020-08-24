namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Bank.Headers

module GetBankAccounts =

  let handler(_: APIGatewayProxyRequest, _: ILambdaContext): APIGatewayProxyResponse =
    let (statusCode, body) =
      match BankAccountDb.getAll with
      | Ok accounts -> 200, Json.serialize { Results = accounts }
      | Error ex -> 500, Json.serialize { Message = ex.Message }

    APIGatewayProxyResponse(
      StatusCode = statusCode,
      Body = body,
      Headers = headers
    )
