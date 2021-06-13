import cls from 'classnames'
import React from 'react'
import { Box, Button, Heading, Level } from 'react-bulma-components'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSyncAlt } from '@fortawesome/free-solid-svg-icons'

import AccountsList from './AccountsList'
import { BankAccount } from '../../types'

const { Item, Side } = Level

type Props = {
  accounts: BankAccount[]
  loading: boolean
  error: boolean
  message: string | undefined
  onRefresh: () => void
}

function AccountsBox(props: Props): JSX.Element {
  const { accounts, loading, error, message, onRefresh } = props
  const faIconClass = cls({ 'fa-spin': loading })
  return (
    <Box>
      <Level mobile>
        <Side align='left'>
          <Heading subtitle size={4} className='has-text-weight-light'>
            Bank accounts
          </Heading>
        </Side>
        <Side align='right'>
          <Item>
            <p className='buttons'>
              <Button
                className='button'
                color='primary'
                disabled={loading}
                inverted
                onClick={onRefresh}
              >
                <span className='icon is-small'>
                  <FontAwesomeIcon className={faIconClass} icon={faSyncAlt} />
                </span>
              </Button>
            </p>
          </Item>
        </Side>
      </Level>
      <AccountsList accounts={accounts} loading={loading} error={error} message={message} />
    </Box>
  )
}

export default AccountsBox
