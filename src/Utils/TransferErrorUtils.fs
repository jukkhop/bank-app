namespace Bank

module TransferErrorUtils =

  let private toReason = function
    | InsufficientFunds -> "InsufficientFunds"
    | DatabaseError _ -> "DatabaseError"
    | OtherError _ -> "OtherError"

  let private toMessage = function
    | InsufficientFunds -> "Insufficient funds on the account"
    | DatabaseError msg | OtherError msg -> msg

  let toHttpStatus = function
    | InsufficientFunds -> HttpStatus.Conflict
    | DatabaseError _ | OtherError _ -> HttpStatus.InternalError

  let toErrorBody (error: TransferError) : ErrorBody =
    { Reason = toReason error
      Message = toMessage error }
