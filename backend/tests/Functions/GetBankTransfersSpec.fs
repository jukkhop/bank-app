namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.BankTransferDb
open Bank.TestData
open Foq
open FsUnit
open Xunit

module GetBankTransfersSpec =

  [<Fact>]
  let ``return all bank transfers`` () =
    let transferDbMock =
      Mock<IBankTransferDb>.With(fun mock ->
        <@ mock.GetAll () --> Ok [transfer1] @>
      )

    let request = APIGatewayProxyRequest()
    let response = GetBankTransfers.impl request transferDbMock

    do response.StatusCode |> should equal 200

    match Json.deserialize<GetBankTransfersResponseDto> response.Body with
    | Ok x -> x.Transfers |> should equal [transfer1]
    | Error _ -> failwith "expected Ok, got an Error"

  [<Fact>]
  let ``return an error if getting the transfers fails`` () =
    let transferDbMock =
      Mock<IBankTransferDb>.With(fun mock ->
        <@ mock.GetAll () --> Error (Failure "some error") @>
      )

    let request = APIGatewayProxyRequest()
    let response = GetBankTransfers.impl request transferDbMock

    do response.StatusCode |> should equal 500

    match Json.deserialize<GenericErrorBody> response.Body with
    | Ok x -> x.Message |> should equal "some error"
    | Error _ -> failwith "expected Ok, got an Error"
