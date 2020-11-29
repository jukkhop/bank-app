namespace Bank

open Amazon.Lambda.APIGatewayEvents
open Bank.AccountOwnerDb
open Bank.TestData
open Foq
open FsUnit
open Xunit

module GetAccountOwnersSpec =

  [<Fact>]
  let ``return all account owners`` () =
    let ownerDbMock =
      Mock<IAccountOwnerDb>.With(fun mock ->
        <@ mock.GetAll () --> Ok [owner1; owner2] @>
      )

    let request = APIGatewayProxyRequest()
    let response = GetAccountOwners.impl request ownerDbMock

    do response.StatusCode |> should equal 200

    match Json.deserialize<GetAccountOwnersResponseDto> response.Body with
    | Ok x -> x.Owners |> should equal [owner1; owner2]
    | Error _ -> failwith "expected Ok, got an Error"

  [<Fact>]
  let ``return an error if fetching the owners fails`` () =
    let ownerDbMock =
      Mock<IAccountOwnerDb>.With(fun mock ->
        <@ mock.GetAll () --> Error (Failure "some error") @>
      )

    let request = APIGatewayProxyRequest()
    let response = GetAccountOwners.impl request ownerDbMock

    do response.StatusCode |> should equal 500

    match Json.deserialize<GenericErrorBody> response.Body with
    | Ok x -> x.Message |> should equal "some error"
    | Error _ -> failwith "expected Ok, got an Error"
