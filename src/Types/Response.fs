namespace Bank

type OkBody<'A> = {
  Results: 'A list
}

type GenericErrorBody = {
  Message: string
}

type SpecificErrorBody = {
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
