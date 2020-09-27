/* eslint-disable @typescript-eslint/no-explicit-any */

import React from 'react'
import { Progress, Table } from 'react-bulma-components'

import { toEuros } from '../../converters'
import { useObservable } from '../../hooks'
import { getAccounts$ } from '../../observables'

function AccountsList(): JSX.Element {
  const data = useObservable(getAccounts$)
  const { accounts = [], loading, error, message } = data || {}

  if (loading) {
    return <Progress color='primary' size='small' />
  }

  if (error) {
    return <p>{`Error occurred: ${message}`}</p>
  }

  return (
    <Table>
      <thead>
        <tr>
          <th>Account number</th>
          <th>Balance</th>
        </tr>
      </thead>
      <tbody>
        {accounts.map((account) => (
          <tr key={account.accountId}>
            <td>{account.accountNumber}</td>
            <td>{toEuros(account.balanceEurCents)}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  )
}

export default AccountsList
