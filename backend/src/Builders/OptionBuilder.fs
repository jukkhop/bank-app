namespace Bank

type OptionBuilder () =
  member __.Bind (opt, binder) =
    match opt with
    | Some value -> binder value
    | None -> None

  member __.Return (value) = Some value

  member __.ReturnFrom (value) = value

[<AutoOpen>]
module OptionBuilder =

  let option = OptionBuilder ()
