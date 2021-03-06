import cls from 'classnames'
import React from 'react'
import { Box, Button, Heading, Level } from 'react-bulma-components'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSyncAlt } from '@fortawesome/free-solid-svg-icons'

import OwnersList from './OwnersList'
import { AccountOwner } from '../../types'

const { Item, Side } = Level

type Props = {
  owners: AccountOwner[]
  loading: boolean
  error: boolean
  message: string | undefined
  onRefresh: () => void
}

function OwnersBox(props: Props): JSX.Element {
  const { owners, loading, error, message, onRefresh } = props
  const faIconClass = cls({ 'fa-spin': loading })
  return (
    <Box>
      <Level mobile>
        <Side align='left'>
          <Heading subtitle size={4} className='has-text-weight-light'>
            Account owners
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
      <OwnersList owners={owners} loading={loading} error={error} message={message} />
    </Box>
  )
}

export default OwnersBox
