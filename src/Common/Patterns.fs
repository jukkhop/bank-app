namespace Bank

module Patterns =

  let (|IsEmpty|NonEmpty|) (list: 'a list) =
    if list.IsEmpty
      then IsEmpty
      else NonEmpty list
