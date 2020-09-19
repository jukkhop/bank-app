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
        |> Json.deserialize<CreateBankTransferDto>
        |> orFailWith parseErrorResponse

      let! validData = validate data |> orFailWith validationErrorResponse

      let transferResult =
        makeTransfer validData.FromAccountId validData.ToAccountId validData.AmountEurCents

      return!
        match transferResult with
        | Ok transfer -> Ok <| successResponse { Transfer = transfer }
        | Error error -> Error <| transferErrorResponse error

    } |> either
