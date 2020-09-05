namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.HttpUtils
open Bank.ResultBuilder

module CreateBankTransfer =

  let private result = ResultBuilder()

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
        | Error err ->
          match err with
          | InsufficientFunds -> Error <| mkSpecificErrorResponse (string err) "Insufficient funds on the account"
          | DatabaseError msg
          | OtherError msg -> Error <| mkSpecificErrorResponse (string err) msg
    }

    response |> ResultBuilder.Flatten
