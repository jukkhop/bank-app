namespace Bank

type ResultBuilder () =
  member __.Bind (value, fn) =
    match value with
    | Ok x -> fn x
    | Error x -> Error x

  member __.Return (value) = Ok value

  member __.ReturnFrom (value) = value

module ResultBuilder =

  let either (value: Result<'a, 'a>) : 'a =
    match value with
    | Ok x -> x
    | Error x -> x

  let isTrueOrFailWith (err: 'a) (cond: bool) : Result<unit, 'a> =
    match cond with
    | true -> Ok ()
    | false -> Error err

  let orFailWithCase (err: string -> 'b) (value: Result<'a, exn>) : Result<'a, 'b> =
    match value with
    | Ok x -> Ok x
    | Error ex -> Error (err ex.Message)

  let continueWith (fn: 'a -> Result<'b, 'c>) (value: Result<'a, 'c>) : Result<'b, 'c> =
    match value with
    | Ok x -> fn x
    | Error x -> Error x

  let orFailWith (fn: 'b -> 'c) (value: Result<'a, 'b>) : Result<'a, 'c> =
    Result.mapError fn value
