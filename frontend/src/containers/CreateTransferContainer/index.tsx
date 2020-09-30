/* eslint-disable @typescript-eslint/no-unused-vars */

import React from 'react'
import { useForm } from 'react-hook-form'

import CreateTransfer from '../../components/CreateTransfer'
import { useObservable } from '../../hooks'
import { createTransfer$, dispatchCreateTransferRequest, getAccounts$ } from '../../observables'
import { CreateBankTransferRequest } from '../../types'

function CreateTransferContainer(): JSX.Element {
  const { errors: formErrors, handleSubmit, register } = useForm()
  const accountsData = useObservable(getAccounts$)
  const transferData = useObservable(createTransfer$)

  const { accounts = [], loading: accountsLoading = false, error: accountsError = false } =
    accountsData || {}

  const { transfer, loading: transferLoading = false, error: transferError = false } =
    transferData || {}

  return (
    <CreateTransfer
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
  const request: CreateBankTransferRequest = {
    fromAccountId: Number(values.fromAccountId),
    toAccountId: Number(values.toAccountId),
    amountEurCents: parseFloat(values.amount) * 100,
  }
  dispatchCreateTransferRequest(request)
}

export default CreateTransferContainer
