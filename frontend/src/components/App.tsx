import React from 'react'
import { Box, Container, Columns, Heading } from 'react-bulma-components'
import 'react-bulma-components/dist/react-bulma-components.min.css'

import AccountsList from './AccountsList'
import OwnersList from './OwnersList'
import TransfersList from './TransfersList'
import CreateTransfer from '../containers/CreateTransferContainer'

const { Column } = Columns

function App(): JSX.Element {
  return (
    <Container className='App'>
      <Box style={{ marginTop: '2rem' }}>
        <Heading subtitle size={4}>
          Make a transfer
        </Heading>
        <CreateTransfer />
      </Box>
      <Columns>
        <Column>
          <Box>
            <Heading subtitle size={4}>
              Bank transfers
            </Heading>
            <TransfersList />
          </Box>
        </Column>
      </Columns>
      <Columns>
        <Column>
          <Box>
            <Heading subtitle size={4}>
              Account owners
            </Heading>
            <OwnersList />
          </Box>
        </Column>
        <Column>
          <Box>
            <Heading subtitle size={4}>
              Bank accounts
            </Heading>
            <AccountsList />
          </Box>
        </Column>
      </Columns>
    </Container>
  )
}

export default App
