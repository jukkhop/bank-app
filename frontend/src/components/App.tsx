import React from 'react'
import { Container, Columns } from 'react-bulma-components'
import 'react-bulma-components/dist/react-bulma-components.min.css'

import AccountsBoxContainer from '../containers/AccountsBoxContainer'
import CreateTransferBox from './CreateTransferBox'
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
