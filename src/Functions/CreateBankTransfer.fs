namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open Bank.HttpUtils
open Bank.ResultBuilder

module CreateBankTransfer =

  let private result = ResultBuilder()

  let handler(req: APIGatewayProxyRequest, _: ILambdaContext): APIGatewayProxyResponse =
    let data = Json.deserialize<CreateBankTransferDto> req.Body

    let response = result {
      let! _ =
        CreateBankTransferValidation.validate data
        |> orFailWithFn mkValidationErrorResponse

      let transferResult =
        BankTransferDb.makeTransfer
          (data.FromAccountId.Value |> AccountId)
          (data.ToAccountId.Value |> AccountId)
          (data.AmountEurCents.Value |> TransferAmount)

      let! resp =
        match transferResult with
        | Ok transfer -> Ok <| mkSuccessResponse [transfer]
        | Error err ->
          match err with
          | InsufficientFunds -> Error <| mkSpecificErrorResponse (string err) "Insufficient funds on the account"
          | DatabaseError msg
          | OtherError msg -> Error <| mkSpecificErrorResponse (string err) msg

      return resp
    }

    response |> ResultBuilder.Flatten
