import React from 'react'
import { Container, Columns } from 'react-bulma-components'
import 'bulma/css/bulma.min.css'

import CreateTransferBox from './Transfers/CreateTransferBox'
import AccountsBoxContainer from '../containers/AccountsBoxContainer'
import OwnersBoxContainer from '../containers/OwnersBoxContainer'
import TransfersBoxContainer from '../containers/TransfersBoxContainer'

const { Column } = Columns

function App(): JSX.Element {
  return (
    <Container className='App'>
      <CreateTransferBox />
      <TransfersBoxContainer />
      <Columns>
        <Column>
          <OwnersBoxContainer />
        </Column>
        <Column>
          <AccountsBoxContainer />
        </Column>
      </Columns>
    </Container>
  )
}

export default App
