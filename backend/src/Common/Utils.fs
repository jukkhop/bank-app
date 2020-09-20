namespace Bank

open Bank.Patterns

module Utils =

  let flip f x y = f y x
  let toList (a, b) = [a; b]

  let getOrFail msg value =
    match value with
    | Some x -> x
    | None -> failwith msg

  let getOrElse def value =
    match value with
    | Some x -> x
    | None -> def

  let nullableToOption (value: obj) : obj option =
    match value with
    | SomeObj x -> Some x
    | _ -> None

  let unitize (res: Result<'a, 'b>) : Result<unit, 'b> =
    match res with
    | Ok _ -> Ok ()
    | Error ex -> Error ex
