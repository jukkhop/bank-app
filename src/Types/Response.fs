namespace Bank

type HttpStatus =
  | Ok = 200
  | BadRequest = 400
  | Conflict = 409
  | InternalError = 500

type OkBody<'a> = {
  Results: 'a list
}

type GenericErrorBody = {
  Message: string
}

type ErrorBody = {
  Reason: string
  Message: string
}

type ValidationError = {
  Field: string
  Message: string
}

type ValidationErrorBody = {
  ValidationErrors: ValidationError list
}
