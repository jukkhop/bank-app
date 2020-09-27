/* eslint-disable @typescript-eslint/no-explicit-any */

import React from 'react'
import { Progress, Table } from 'react-bulma-components'

import { fullName } from '../../converters'
import { useObservable } from '../../hooks'
import { getOwners$ } from '../../observables'

function OwnersList(): JSX.Element {
  const data = useObservable(getOwners$)
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
