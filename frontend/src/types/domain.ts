type AccountOwner = {
  ownerId: number
  firstName: string
  middleName?: string
  lastName: string
  nationality: string
  dateOfBirth: string
}

type BankAccount = {
  accountId: number
  owner: AccountOwner
  accountNumber: string
  balanceEurCents: number
}

type BankTransfer = {
  transferId: number
  createdAt: string
  fromAccount: BankAccount
  toAccount: BankAccount
  amountEurCents: number
}

export type { AccountOwner, BankAccount, BankTransfer }
