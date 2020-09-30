/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable jsx-a11y/label-has-associated-control */

import React from 'react'
import { Columns, Button } from 'react-bulma-components'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {
  faEuroSign,
  faMoneyCheckAlt,
  faExclamationTriangle,
} from '@fortawesome/free-solid-svg-icons'
import { BankAccount } from '../../types'

const { Column } = Columns

const columnStyle = {
  flexBasis: 'initial',
  flexGrow: 0,
  flexShrink: 0,
}

type CreateTransferProps = {
  accounts: BankAccount[]
  accountsError: boolean
  accountsLoading: boolean
  formErrors: any
  onCreateTransfer: () => void
  register: (args: any) => any
  transferLoading: boolean
}

function CreateTransfer({
  accounts = [],
  accountsError,
  accountsLoading,
  formErrors,
  onCreateTransfer,
  register,
  transferLoading,
}: CreateTransferProps): JSX.Element {
  // prettier-ignore
  const accountSpanClass =
    `select ${accountsLoading ? 'is-loading' : ''} ${accountsError ? 'is-danger' : ''}`
  const amountControlClass = formErrors.amount ? 'has-icons-right' : ''
  const amountInputClass = formErrors.amount ? 'is-danger' : ''
  const submitButtonClass = transferLoading ? 'is-loading' : ''
  return (
    <form onSubmit={onCreateTransfer}>
      <Columns>
        <Column style={columnStyle}>
          <div className='field'>
            <label className='label' htmlFor='fromAccountId'>
              From account
            </label>
            <div className='control has-icons-left'>
              <span className={accountSpanClass}>
                <select name='fromAccountId' ref={register}>
                  {accounts.map(accountToOption)}
                </select>
              </span>
              <span className='icon is-left'>
                <FontAwesomeIcon icon={faMoneyCheckAlt} />
              </span>
              {accountsError && <p className='help is-danger'>Error fetching accounts</p>}
            </div>
          </div>
        </Column>
        <Column style={columnStyle}>
          <div className='field'>
            <label className='label' htmlFor='toAccountId'>
              To account
            </label>
            <div className='control has-icons-left'>
              <span className={accountSpanClass}>
                <select name='toAccountId' ref={register}>
                  {accounts.map(accountToOption)}
                </select>
              </span>
              <span className='icon is-left'>
                <FontAwesomeIcon icon={faMoneyCheckAlt} />
              </span>
              {accountsError && <p className='help is-danger'>Error fetching accounts</p>}
            </div>
          </div>
        </Column>
        <Column style={columnStyle}>
          <div className='field'>
            <label className='label' htmlFor='amount'>
              Amount
            </label>
            <div className={`control has-icons-left ${amountControlClass}`}>
              <input
                className={`input ${amountInputClass}`}
                defaultValue={0}
                name='amount'
                ref={register({ required: true, min: 1, max: 999999999 })}
                type='number'
              />
              <span className='icon is-left'>
                <FontAwesomeIcon icon={faEuroSign} />
              </span>
              {formErrors.amount && (
                <span className='icon is-small is-right'>
                  <FontAwesomeIcon icon={faExclamationTriangle} />
                </span>
              )}
              {formErrors.amount && <p className='help is-danger'>Amount is invalid</p>}
            </div>
          </div>
        </Column>
        <Column>
          <div className='field'>
            <label className='label' htmlFor='amount'>
              &nbsp;
            </label>
            <div className='control'>
              <Button className={submitButtonClass} color='primary' type='submit'>
                Transfer
              </Button>
            </div>
          </div>
        </Column>
      </Columns>
    </form>
  )
}

function accountToOption(account: BankAccount): JSX.Element {
  return (
    <option key={account.accountId} value={account.accountId}>
      {account.accountNumber}
    </option>
  )
}

export default CreateTransfer