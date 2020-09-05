namespace Bank

module Extractors =

  let txrErrorReason = function
    | InsufficientFunds -> "InsufficientFunds"
    | DatabaseError _ -> "DatabaseError"
    | OtherError _ -> "OtherError"

  let txrErrorMsg = function
    | InsufficientFunds -> "Insufficient funds on the account"
    | DatabaseError msg | OtherError msg -> msg
