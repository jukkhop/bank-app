/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-explicit-any */

import { Observable, Subject, of, Observer } from 'rxjs'
import * as RxFetch from 'rxjs/fetch'
import { catchError, startWith, switchMap } from 'rxjs/operators'
import {
  CreateBankTransferRequest,
  CreateBankTransferResponse,
  GetAccountOwnersResponse,
  GetBankAccountsResponse,
  GetBankTransfersResponse,
} from '../types'
import { mkEndpointUrl } from '../utils'

function fromFetch<T>(url: string, init?: RequestInit): Observable<T> {
  return RxFetch.fromFetch(url, init).pipe(
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
      return of({ loading: false, error: true, message: err.message })
    }),
  ) as Observable<T>
}

function fetchAndObserve<T>(url: string, init: RequestInit, observer: Observer<T>): void {
  fromFetch<T>(url, init).subscribe(
    (value) => observer.next(value),
    (err) => observer.error(err),
  )
}

const getOwners$ = fromFetch<GetAccountOwnersResponse>(mkEndpointUrl('get-account-owners'))
const getAccounts$ = fromFetch<GetBankAccountsResponse>(mkEndpointUrl('get-bank-accounts'))
const getTransfers$ = fromFetch<GetBankTransfersResponse>(mkEndpointUrl('get-bank-transfers'))

const createTransferSubject$ = new Subject<CreateBankTransferResponse>()
const createTransfer$ = createTransferSubject$.asObservable()

function dispatchCreateTransferRequest(request: CreateBankTransferRequest): void {
  fetchAndObserve<CreateBankTransferResponse>(
    mkEndpointUrl('create-bank-transfer'),
    {
      body: JSON.stringify(request),
      method: 'POST',
    },
    createTransferSubject$,
  )
}

export {
  createTransfer$,
  dispatchCreateTransferRequest,
  fromFetch,
  getAccounts$,
  getOwners$,
  getTransfers$,
}
