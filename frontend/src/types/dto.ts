import { AccountOwner, BankAccount, BankTransfer } from './domain'

type ResponseCommon = {
  loading?: boolean
  error?: boolean
  message?: string
}

type GetAccountOwnersResponse = ResponseCommon & { owners?: AccountOwner[] }
type GetBankAccountsResponse = ResponseCommon & { accounts?: BankAccount[] }
type GetBankTransfersResponse = ResponseCommon & { transfers?: BankTransfer[] }

type CreateBankTransferRequest = {
  fromAccountId: number
  toAccountId: number
  amountEurCents: number
}

type CreateBankTransferResponse = ResponseCommon & { transfer?: BankTransfer }

export type {
  GetAccountOwnersResponse,
  GetBankAccountsResponse,
  GetBankTransfersResponse,
  CreateBankTransferRequest,
  CreateBankTransferResponse,
}
