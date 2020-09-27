/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable jsx-a11y/label-has-associated-control */

import React from 'react'
import { Columns, Button } from 'react-bulma-components'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faEuroSign, faMoneyCheckAlt } from '@fortawesome/free-solid-svg-icons'
import { BankAccount } from '../../types'

const { Column } = Columns

const columnStyle = {
  flexBasis: 'initial',
  flexGrow: 0,
  flexShrink: 0,
}

function accountToOption(account: BankAccount): JSX.Element {
  return (
    <option key={account.accountId} value={account.accountId}>
      {account.accountNumber}
    </option>
  )
}

type CreateTransferProps = {
  accounts: BankAccount[]
  accountsLoading: boolean
  errors: any
  onCreateTransfer: () => void
  register: () => void
  transferLoading: boolean
}

function CreateTransfer({
  accounts = [],
  accountsLoading,
  errors,
  onCreateTransfer,
  register,
  transferLoading,
}: CreateTransferProps): JSX.Element {
  const selectClass = accountsLoading ? 'select is-loading' : 'select'
  const buttonClass = transferLoading ? 'is-loading' : ''
  return (
    <form onSubmit={onCreateTransfer}>
      <Columns>
        <Column style={columnStyle}>
          <div className='field'>
            <label className='label' htmlFor='fromAccountId'>
              From account
            </label>
            <div className='control has-icons-left'>
              <span className={selectClass}>
                <select name='fromAccountId' ref={register}>
                  {accounts.map(accountToOption)}
                </select>
              </span>
              <span className='icon is-left'>
                <FontAwesomeIcon icon={faMoneyCheckAlt} />
              </span>
            </div>
          </div>
        </Column>
        <Column style={columnStyle}>
          <div className='field'>
            <label className='label' htmlFor='toAccountId'>
              To account
            </label>
            <div className='control has-icons-left'>
              <span className={selectClass}>
                <select name='toAccountId' ref={register}>
                  {accounts.map(accountToOption)}
                </select>
              </span>
              <span className='icon is-left'>
                <FontAwesomeIcon icon={faMoneyCheckAlt} />
              </span>
            </div>
          </div>
        </Column>
        <Column style={columnStyle}>
          <div className='field'>
            <label className='label' htmlFor='amount'>
              Amount
            </label>
            <div className='control has-icons-left'>
              <input
                className='input'
                defaultValue={0}
                name='amount'
                ref={register}
                type='number'
              />
              <span className='icon is-left'>
                <FontAwesomeIcon icon={faEuroSign} />
              </span>
            </div>
          </div>
        </Column>
        <Column style={{ alignSelf: 'flex-end' }}>
          <div className='field'>
            <div className='control'>
              <Button className={buttonClass} color='primary' type='submit'>
                Transfer
              </Button>
            </div>
          </div>
        </Column>
      </Columns>
    </form>
  )
}

export default CreateTransfer
