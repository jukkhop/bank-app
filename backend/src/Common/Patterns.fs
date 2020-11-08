namespace Bank

module Patterns =

  let (|SomeObj|_|) =
    let td = typedefof<option<_>>
    fun (a: obj) ->
      let t = a.GetType()
      let value = t.GetProperty("Value")
      if t.IsGenericType && t.GetGenericTypeDefinition() = td then
        if isNull a then None
        else Some <| value.GetValue(a, [| |])
      else None
