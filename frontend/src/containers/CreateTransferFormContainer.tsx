/* eslint-disable @typescript-eslint/no-unused-vars */

import React from 'react'
import { useForm } from 'react-hook-form'
import { takeWhile } from 'rxjs/operators'

import CreateTransferForm from '../components/Transfers/CreateTransferForm'
import { useObservable } from '../hooks'
import { initials, sinks, sources } from '../store'
import { CreateTransferRequest, GetAccountsResponse } from '../types'

const initialAccounts$ = sources.getAccounts.pipe(
  takeWhile((x: GetAccountsResponse) => x.loading, true),
)

function CreateTransferFormContainer(): JSX.Element {
  const { errors: formErrors, handleSubmit, register } = useForm()
  const accountsData = useObservable(initialAccounts$, initials.getAccounts)
  const transferData = useObservable(sources.createTransfer, initials.createTransfer)

  // prettier-ignore
  const { accounts = [], loading: accountsLoading = false, error: accountsError = false } =
    accountsData || {}

  // prettier-ignore
  const { transfer, loading: transferLoading = false, error: transferError = false } =
    transferData || {}

  return (
    <CreateTransferForm
      accounts={accounts}
      accountsError={accountsError}
      accountsLoading={accountsLoading}
      formErrors={formErrors}
      onCreateTransfer={handleSubmit(onCreateTransfer)}
      register={register}
      transferLoading={transferLoading}
    />
  )
}

type FormValues = {
  fromAccountId: string
  toAccountId: string
  amount: string
}

function onCreateTransfer(values: FormValues): void {
  const request: CreateTransferRequest = {
    fromAccountId: parseInt(values.fromAccountId, 10),
    toAccountId: parseInt(values.toAccountId, 10),
    amountEurCents: parseFloat(values.amount) * 100,
  }
  sinks.createTransfer.next(request)
}

export default CreateTransferFormContainer
