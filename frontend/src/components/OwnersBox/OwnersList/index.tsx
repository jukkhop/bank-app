import React from 'react'
import { Progress, Table } from 'react-bulma-components'

import { fullName } from '../../../converters'
import { useObservable } from '../../../hooks'
import { initials, sources } from '../../../store'

function OwnersList(): JSX.Element {
  const data = useObservable(sources.getOwners, initials.getOwners)
  const { owners = [], loading, error, message } = data || {}

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
        </tr>
      </thead>
      <tbody>
        {owners.map((owner) => (
          <tr key={owner.ownerId}>
            <td>{fullName(owner)}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  )
}

export default OwnersList
