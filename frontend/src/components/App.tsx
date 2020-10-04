import React from 'react'
import { Container, Columns } from 'react-bulma-components'
import 'react-bulma-components/dist/react-bulma-components.min.css'

import AccountsBox from './AccountsBox'
import OwnersBox from './OwnersBox'
import TransfersBox from './TransfersBox'
import CreateTransferBox from './CreateTransferBox'

const { Column } = Columns

function App(): JSX.Element {
  return (
    <Container className='App'>
      <CreateTransferBox />
      <TransfersBox />
      <Columns>
        <Column>
          <OwnersBox />
        </Column>
        <Column>
          <AccountsBox />
        </Column>
      </Columns>
    </Container>
  )
}

export default App
