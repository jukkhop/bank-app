import { Observable, Subject, BehaviorSubject, merge, timer } from 'rxjs'
import { delay, map, switchMap } from 'rxjs/operators'

import { sourceSubjects } from './sources'
import { CreateTransferRequest, Methods } from '../types'
import { mkEndpointUrl, observeFetch } from '../utils'

enum Endpoints {
  createTransfer = 'create-bank-transfer',
  getAccounts = 'get-bank-accounts',
  getOwners = 'get-account-owners',
  getTransfers = 'get-bank-transfers',
}

enum ActionTypes {
  createTransfer = 'CREATE_TRANSFER',
  getAccounts = 'GET_ACCOUNTS',
  getOwners = 'GET_OWNERS',
  getTransfers = 'GET_TRANSFERS',
}

type Action<T> = {
  type: ActionTypes
  method: Methods
  endpoint: Endpoints
  data?: T
}

const subjects = {
  createTransfer: new Subject<CreateTransferRequest>(),
  getAccounts: new BehaviorSubject<never>(null as never),
  getOwners: new BehaviorSubject<never>(null as never),
  getTransfers: new BehaviorSubject<never>(null as never),
}

const actions = {
  createTransfer(request: CreateTransferRequest): Action<CreateTransferRequest> {
    return {
      type: ActionTypes.createTransfer,
      method: Methods.post,
      endpoint: Endpoints.createTransfer,
      data: request,
    }
  },
  getAccounts(): Action<never> {
    return {
      type: ActionTypes.getAccounts,
      method: Methods.get,
      endpoint: Endpoints.getAccounts,
    }
  },
  getOwners(): Action<never> {
    return {
      type: ActionTypes.getOwners,
      method: Methods.get,
      endpoint: Endpoints.getOwners,
    }
  },
  getTransfers(): Action<never> {
    return {
      type: ActionTypes.getTransfers,
      method: Methods.get,
      endpoint: Endpoints.getTransfers,
    }
  },
}

const polling = {
  getAccounts: subjects.getAccounts.pipe(switchMap(() => timer(0, 9500))),
  getOwners: subjects.getOwners.pipe(switchMap(() => timer(0, 10000))),
  getTransfers: subjects.getTransfers.pipe(switchMap(() => timer(0, 10500))),
}

const action$: Observable<Action<unknown>> = merge(
  subjects.createTransfer.pipe(map(actions.createTransfer)),
  polling.getAccounts.pipe(map(actions.getAccounts)),
  polling.getOwners.pipe(map(actions.getOwners)),
  polling.getTransfers.pipe(map(actions.getTransfers)),
)

action$.subscribe((action: Action<unknown>) => {
  const url = mkEndpointUrl(action.endpoint)
  const init = {
    method: action.method,
    body: action.data ? JSON.stringify(action.data) : undefined,
  }
  switch (action.type) {
    case ActionTypes.createTransfer: {
      observeFetch(url, init, sourceSubjects.createTransfer)
      break
    }
    case ActionTypes.getAccounts: {
      observeFetch(url, init, sourceSubjects.getAccounts)
      break
    }
    case ActionTypes.getOwners: {
      observeFetch(url, init, sourceSubjects.getOwners)
      break
    }
    case ActionTypes.getTransfers: {
      observeFetch(url, init, sourceSubjects.getTransfers)
      break
    }
    default: {
      break
    }
  }

  sourceSubjects.createTransfer.pipe(delay(1000)).subscribe((resp) => {
    if (!resp.loading && !resp.error) {
      subjects.getTransfers.next(null as never)
      subjects.getAccounts.next(null as never)
    }
  })
})

export { subjects as sinks }
