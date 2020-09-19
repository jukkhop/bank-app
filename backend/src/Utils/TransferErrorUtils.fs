namespace Bank

module TransferErrorUtils =

  let private toHttpStatus = function
    | InsufficientFunds -> HttpStatus.Conflict
    | DatabaseError _ | OtherError _ -> HttpStatus.InternalError

  let private toReason = function
    | InsufficientFunds -> "INSUFFICIENT_FUNDS"
    | DatabaseError _ -> "DATABASE_ERROR"
    | OtherError _ -> "OTHER_ERROR"

  let private toMessage = function
    | InsufficientFunds -> "Insufficient funds on the account"
    | DatabaseError msg | OtherError msg -> msg

  let private toErrorBody (error: TransferError) =
    { Reason = toReason error
      Message = toMessage error }

  let transferErrorResponse (error: TransferError) =
    HttpUtils.errorResponse (toHttpStatus error) (toErrorBody error)
