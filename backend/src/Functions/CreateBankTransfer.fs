namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.BankTransferDb
open Bank.CreateBankTransferValidation
open Bank.HttpUtils
open Bank.ResultBuilder
open Bank.TransferErrorUtils

module CreateBankTransfer =

  let private result = ResultBuilder ()

  let handler (req: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    result {
      let! data =
        req.Body
        |> deserialize<CreateBankTransferDto>
        |> continueWith validate
        |> orFailWith validationErrorResponse

      let transferResult = makeTransfer data.FromAccountId data.ToAccountId data.AmountEurCents

      return!
        match transferResult with
        | Ok transfer -> Ok <| successResponse { Transfer = transfer }
        | Error error -> Error <| transferErrorResponse error

    } |> either
