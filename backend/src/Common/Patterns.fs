namespace Bank

module Patterns =

  let (|SomeObj|_|) =
    let ty = typedefof<option<_>>
    fun (a: obj) ->
      let aty = a.GetType()
      let v = aty.GetProperty("Value")
      if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
        if isNull a then None
        else Some <| v.GetValue(a, [| |])
      else None
