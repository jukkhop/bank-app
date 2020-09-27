import { Observable, Subject, of } from 'rxjs'
import { fromFetch } from 'rxjs/fetch'
import { catchError, startWith, switchMap } from 'rxjs/operators'
import {
  CreateBankTransferRequest,
  CreateBankTransferResponse,
  GetAccountOwnersResponse,
  GetBankAccountsResponse,
  GetBankTransfersResponse,
} from '../types'
import { mkEndpointUrl } from '../utils'

function createObservable<T>(url: string, init?: RequestInit): Observable<T> {
  const observable = fromFetch(url, init).pipe(
    switchMap((response: Response) => {
      if (response.ok) {
        return response.json()
      }
      return of({
        error: true,
        message: response.statusText,
      })
    }),
    startWith({ loading: true }),
    catchError((err: Error) => {
      return of({
        error: true,
        message: err.message,
      })
    }),
  )

  return observable
}

const getOwners$ = createObservable<GetAccountOwnersResponse>(mkEndpointUrl('get-account-owners'))
const getAccounts$ = createObservable<GetBankAccountsResponse>(mkEndpointUrl('get-bank-accounts'))
const getTransfers$ = createObservable<GetBankTransfersResponse>(
  mkEndpointUrl('get-bank-transfers'),
)

const createTransferSubject = new Subject<CreateBankTransferResponse>()
const createTransfer$ = createTransferSubject.asObservable()

async function createTransfer(request: CreateBankTransferRequest): Promise<void> {
  createTransferSubject.next({ loading: true, error: false })

  const httpResponse = await fetch(mkEndpointUrl('create-bank-transfer'), {
    body: JSON.stringify(request),
    method: 'POST',
  })

  let response: CreateBankTransferResponse = { loading: false, error: false }

  if (httpResponse.ok) {
    const { transfer }: CreateBankTransferResponse = await httpResponse.json()
    response = { ...response, transfer }
  } else {
    response = { ...response, error: true, message: httpResponse.statusText }
  }

  createTransferSubject.next(response)
}

export { getOwners$, getAccounts$, getTransfers$, createTransfer$, createTransfer }
