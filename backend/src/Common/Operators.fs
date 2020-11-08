namespace Bank

module Operators =

  let inline (>=<) a (b, c) = a >= b && a <= c
