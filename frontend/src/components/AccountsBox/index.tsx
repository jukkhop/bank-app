import React from 'react'
import { Box, Button, Heading, Level } from 'react-bulma-components'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSyncAlt } from '@fortawesome/free-solid-svg-icons'

import AccountsList from './AccountsList'
import { sinks } from '../../store'

const { Item, Side } = Level

function App(): JSX.Element {
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
                inverted
                onClick={() => sinks.getAccounts.next(null as never)}
              >
                <span className='icon is-small'>
                  <FontAwesomeIcon icon={faSyncAlt} />
                </span>
              </Button>
            </p>
          </Item>
        </Side>
      </Level>
      <AccountsList />
    </Box>
  )
}

export default App
