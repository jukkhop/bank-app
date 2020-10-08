import React from 'react'
import TransfersBox from '../../components/TransfersBox'
import { useObservable } from '../../hooks'
import { initials, sinks, sources } from '../../store'

function TransfersBoxContainer(): JSX.Element {
  const data = useObservable(sources.getTransfers, initials.getTransfers)
  const { transfers = [], loading = false, error = false, message = null } = data || {}

  return (
    <TransfersBox
      transfers={transfers}
      error={error}
      loading={loading}
      message={message}
      onRefresh={onRefresh}
    />
  )
}

function onRefresh(): void {
  sinks.getTransfers.next(null as never)
}

export default TransfersBoxContainer
