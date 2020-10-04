import React from 'react'
import { Box, Button, Heading, Level } from 'react-bulma-components'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSyncAlt } from '@fortawesome/free-solid-svg-icons'

import OwnersList from './OwnersList'
import { sinks } from '../../store'

const { Item, Side } = Level

function OwnersBox(): JSX.Element {
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
                inverted
                onClick={() => sinks.getOwners.next(null as never)}
              >
                <span className='icon is-small'>
                  <FontAwesomeIcon icon={faSyncAlt} />
                </span>
              </Button>
            </p>
          </Item>
        </Side>
      </Level>
      <OwnersList />
    </Box>
  )
}

export default OwnersBox
