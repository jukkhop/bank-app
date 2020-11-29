namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.BankAccountDb
open Bank.TestData
open Foq
open FsUnit
open Xunit

module GetBankAccountsSpec =

  [<Fact>]
  let ``return all bank accounts`` () =
    let accountDbMock =
      Mock<IBankAccountDb>.With(fun mock ->
        <@ mock.GetAll () --> Ok [account1; account2] @>
      )

    let request = APIGatewayProxyRequest()
    let response = GetBankAccounts.impl request accountDbMock

    do response.StatusCode |> should equal 200

    match Json.deserialize<GetBankAccountsResponseDto> response.Body with
    | Ok x -> x.Accounts |> should equal [account1; account2]
    | Error _ -> failwith "expected Ok, got an Error"

  [<Fact>]
  let ``return an error if getting the accounts fails`` () =
    let accountDbMock =
      Mock<IBankAccountDb>.With(fun mock ->
        <@ mock.GetAll () --> Error (Failure "some error") @>
      )

    let request = APIGatewayProxyRequest()
    let response = GetBankAccounts.impl request accountDbMock

    do response.StatusCode |> should equal 500

    match Json.deserialize<GenericErrorBody> response.Body with
    | Ok x -> x.Message |> should equal "some error"
    | Error _ -> failwith "expected Ok, got an Error"
