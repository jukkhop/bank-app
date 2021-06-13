import React from 'react'
import OwnersBox from '../components/Owners/OwnersBox'
import { useObservable } from '../hooks'
import { initials, sinks, sources } from '../store'

function OwnersBoxContainer(): JSX.Element {
  const data = useObservable(sources.getOwners, initials.getOwners)
  const { owners = [], loading = false, error = false, message } = data || {}

  return (
    <OwnersBox
      owners={owners}
      error={error}
      loading={loading}
      message={message}
      onRefresh={onRefresh}
    />
  )
}

function onRefresh(): void {
  sinks.getOwners.next(null as never)
}

export default OwnersBoxContainer
