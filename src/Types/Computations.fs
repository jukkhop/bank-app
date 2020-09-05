namespace Bank

type ResultBuilder() =
  member this.Bind(value, fn) =
    match value with
    | Ok x -> fn x
    | Error x -> Error x

  member this.Return value = Ok value

  static member Flatten value =
    match value with
    | Ok x -> x
    | Error x -> x

module ResultBuilder =

  let isTrueOrFailWith (err: 'a) (cond: bool) =
    match cond with
    | true -> Ok ()
    | false -> Error err

  let orFailWith (err: string -> 'a) (value: Result<'b, exn>) =
    match value with
    | Ok x -> Ok x
    | Error ex -> Error (err ex.Message)

  let orFailWithFn (fn: 'a -> 'b) (value: Result<'c, 'a>) =
    match value with
    | Ok x -> Ok x
    | Error x -> Error (fn x)
