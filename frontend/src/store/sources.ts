import { Subject } from 'rxjs'

import {
  CreateTransferResponse,
  GetOwnersResponse,
  GetAccountsResponse,
  GetTransfersResponse,
} from '../types'

const subjects = {
  createTransfer: new Subject<CreateTransferResponse>(),
  getAccounts: new Subject<GetAccountsResponse>(),
  getOwners: new Subject<GetOwnersResponse>(),
  getTransfers: new Subject<GetTransfersResponse>(),
}

const observables = {
  createTransfer: subjects.createTransfer.asObservable(),
  getAccounts: subjects.getAccounts.asObservable(),
  getOwners: subjects.getOwners.asObservable(),
  getTransfers: subjects.getTransfers.asObservable(),
}

const initials = {
  createTransfer: { loading: false, error: false } as CreateTransferResponse,
  getAccounts: { loading: true, error: false } as GetAccountsResponse,
  getOwners: { loading: true, error: false } as GetOwnersResponse,
  getTransfers: { loading: true, error: false } as GetTransfersResponse,
}

export { initials, observables as sources, subjects as sourceSubjects }
