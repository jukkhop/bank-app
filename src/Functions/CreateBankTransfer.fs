namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.Extractors
open Bank.HttpUtils
open Bank.ResultBuilder

module CreateBankTransfer =

  let private result = ResultBuilder ()

  let handler(req: APIGatewayProxyRequest): APIGatewayProxyResponse =
    let data = Json.deserialize<CreateBankTransferDto> req.Body

    let response = result {
      do! CreateBankTransferValidation.validate data
          |> orFailWithFn mkValidationErrorResponse

      let transferResult =
        BankTransferDb.makeTransfer
          (data.FromAccountId.Value |> AccountId)
          (data.ToAccountId.Value |> AccountId)
          (data.AmountEurCents.Value |> TransferAmount)

      return!
        match transferResult with
        | Ok transfer -> Ok <| mkSuccessResponse [transfer]
        | Error err -> Error <| mkErrorResponse (string err) (extTransferError err)
    }

    response |> ResultBuilder.Flatten
