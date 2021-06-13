import React from 'react'
import AccountsBox from '../components/Accounts/AccountsBox'
import { useObservable } from '../hooks'
import { initials, sinks, sources } from '../store'

function AccountsBoxContainer(): JSX.Element {
  const data = useObservable(sources.getAccounts, initials.getAccounts)
  const { accounts = [], loading = false, error = false, message } = data || {}

  return (
    <AccountsBox
      accounts={accounts}
      error={error}
      loading={loading}
      message={message}
      onRefresh={onRefresh}
    />
  )
}

function onRefresh(): void {
  sinks.getAccounts.next(null as never)
}

export default AccountsBoxContainer
