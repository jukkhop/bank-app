namespace Bank

module Patterns =

  let (|Empty|NotEmpty|) (list: 'a list) =
    if list.IsEmpty
      then Empty
      else NotEmpty list
