import { AccountOwner, BankAccount, BankTransfer } from './domain'

type ResponseCommon = {
  loading: boolean
  error: boolean
  message?: string
}

type GetOwnersResponse = ResponseCommon & { owners?: AccountOwner[] }
type GetAccountsResponse = ResponseCommon & { accounts?: BankAccount[] }
type GetTransfersResponse = ResponseCommon & { transfers?: BankTransfer[] }

type CreateTransferRequest = {
  fromAccountId: number
  toAccountId: number
  amountEurCents: number
}

type CreateTransferResponse = ResponseCommon & { transfer?: BankTransfer }

export type {
  GetOwnersResponse,
  GetAccountsResponse,
  GetTransfersResponse,
  CreateTransferRequest,
  CreateTransferResponse,
}
