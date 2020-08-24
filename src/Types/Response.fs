namespace Bank

type OkBody<'A> = {
  Results: 'A list
}

type ErrorBody = {
  Message: string
}