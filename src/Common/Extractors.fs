namespace Bank

module Extractors =

  let extTransferError = function
    | InsufficientFunds -> "Insufficient funds on the account"
    | DatabaseError msg | OtherError msg -> msg
