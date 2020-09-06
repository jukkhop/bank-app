namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.HttpUtils
open Bank.ResultBuilder
open Bank.TransferErrorUtils

module CreateBankTransfer =

  let private result = ResultBuilder ()

  let handler (req: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    let data = Json.deserialize<CreateBankTransferDto> req.Body

    result {
      do! CreateBankTransferValidation.validate data
          |> Result.mapError mkValidationErrorResponse

      let transferResult =
        BankTransferDb.makeTransfer
          (data.FromAccountId.Value |> AccountId)
          (data.ToAccountId.Value |> AccountId)
          (data.AmountEurCents.Value |> TransferAmount)

      return!
        match transferResult with
        | Ok transfer -> Ok <| mkSuccessResponse { Transfer = transfer }
        | Error error -> Error <| mkErrorResponse (toHttpStatus error) (toErrorBody error)

    } |> either
