namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.BankAccountDb
open Bank.BankTransferDb
open Bank.CreateBankTransferValidation
open Bank.HttpUtils
open Bank.TransferErrorUtils
open Bank.TransferService

module CreateBankTransfer =

  let impl (request: APIGatewayProxyRequest) (service: ITransferService) : APIGatewayProxyResponse =
    result {
      let! data =
        request.Body
        |> Json.deserialize<CreateBankTransferDto>
        |> orFailWith parseErrorResponse

      let! validData = validate service.AccountDb data |> orFailWith validationErrorResponse

      let transferResult =
        service.MakeTransfer validData.FromAccountId validData.ToAccountId validData.AmountEurCents

      return!
        match transferResult with
        | Ok transfer -> Ok <| successResponse { Transfer = transfer }
        | Error error -> Error <| transferErrorResponse error

    } |> either

  let handler (request: APIGatewayProxyRequest) : APIGatewayProxyResponse =
    let accountDb = BankAccountDb ()
    let transferDb = BankTransferDb ()
    let service = TransferService (accountDb, transferDb)
    impl request (service :> ITransferService)
