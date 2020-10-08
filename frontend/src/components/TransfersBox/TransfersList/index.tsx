import React from 'react'
import { Progress, Table } from 'react-bulma-components'
import { fullName, showDate, toEuros } from '../../../converters'
import { BankTransfer } from '../../../types'

type Props = {
  transfers: BankTransfer[]
  loading: boolean
  error: boolean
  message: string | null
}

function TransfersList({ transfers, loading, error, message = null }: Props): JSX.Element {
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
          <th>Time</th>
          <th>From</th>
          <th>To</th>
          <th>Amount</th>
        </tr>
      </thead>
      <tbody>
        {transfers.map((transfer) => (
          <tr key={transfer.transferId}>
            <td>{showDate(transfer.createdAt)}</td>
            <td>{fullName(transfer.fromAccount.owner)}</td>
            <td>{fullName(transfer.toAccount.owner)}</td>
            <td>{toEuros(transfer.amountEurCents)}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  )
}

export default TransfersList
