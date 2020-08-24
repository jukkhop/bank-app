namespace Bank

module Utils =

  let flip f x y = f y x
  let toList (a, b) = [a; b]

  let getOrFail value msg =
    match value with
    | Some x -> x
    | None -> failwith msg

  let getOrElse def value =
    match value with
    | Some x -> x
    | None -> def
