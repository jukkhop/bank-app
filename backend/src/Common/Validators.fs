namespace Bank

module Validators =

  let requiredValidator (field: string) (value: obj option) : ValidationError list =
    match value with
    | Some _ -> List.empty
    | None -> [{ Field = field; Message = "Field is required" }]
