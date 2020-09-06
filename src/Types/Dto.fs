namespace Bank

type CreateBankTransferDto = {
  FromAccountId: int64 option
  ToAccountId: int64 option
  AmountEurCents: int64 option
}

type CreateBankTransferResponseDto = {
  Transfer: BankTransfer
}
