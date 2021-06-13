import React from 'react'
import { Progress, Table } from 'react-bulma-components'
import { fullName } from '../../converters'
import { AccountOwner } from '../../types'

type Props = {
  owners: AccountOwner[]
  loading: boolean
  error: boolean
  message: string | undefined
}

function OwnersList(props: Props): JSX.Element {
  const { owners, loading, error, message } = props

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
          <th>Name</th>
          <th>Nationality</th>
        </tr>
      </thead>
      <tbody>
        {owners.map((owner) => (
          <tr key={owner.ownerId}>
            <td>{fullName(owner)}</td>
            <td>{owner.nationality}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  )
}

export default OwnersList
