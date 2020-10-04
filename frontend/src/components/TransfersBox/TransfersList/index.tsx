import React from 'react'
import { Progress, Table } from 'react-bulma-components'

import { fullName, showDate, toEuros } from '../../../converters'
import { useObservable } from '../../../hooks'
import { initials, sources } from '../../../store'

function TransfersList(): JSX.Element {
  const data = useObservable(sources.getTransfers, initials.getTransfers)
  const { transfers = [], loading, error, message } = data || {}

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
