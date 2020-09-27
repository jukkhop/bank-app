/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */

import React from 'react'
import { useForm } from 'react-hook-form'

import CreateTransfer from '../../components/CreateTransfer'
import { useObservable } from '../../hooks'
import { createTransfer, createTransfer$, getAccounts$ } from '../../observables'

async function onCreateTransfer(values: any) {
  createTransfer({
    fromAccountId: Number(values.fromAccountId),
    toAccountId: Number(values.toAccountId),
    amountEurCents: parseFloat(values.amount) * 100,
  })
}

function CreateTransferContainer(): JSX.Element {
  const { errors, handleSubmit, register } = useForm()
  const accountsData = useObservable(getAccounts$)
  const transferData = useObservable(createTransfer$)

  const { accounts = [], loading: accountsLoading = false } = accountsData || {}
  const { transfer, loading: transferLoading = false } = transferData || {}

  return (
    <CreateTransfer
      accounts={accounts}
      accountsLoading={accountsLoading}
      errors={errors}
      onCreateTransfer={handleSubmit(onCreateTransfer)}
      register={register}
      transferLoading={transferLoading}
    />
  )
}

export default CreateTransferContainer
