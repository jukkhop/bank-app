import React from 'react'
import { Progress, Table } from 'react-bulma-components'
import { toEuros } from '../../converters'
import { BankAccount } from '../../types'

type Props = {
  accounts: BankAccount[]
  loading: boolean
  error: boolean
  message: string | undefined
}

function AccountsList(props: Props): JSX.Element {
  const { accounts, loading, error, message } = props

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
