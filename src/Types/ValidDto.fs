namespace Bank

type ValidCreateBankTransferDto = {
  FromAccountId: AccountId
  ToAccountId: AccountId
  AmountEurCents: TransferAmount
}
