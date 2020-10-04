import React from 'react'
import { Box, Heading } from 'react-bulma-components'
import CreateTransferForm from '../../containers/CreateTransferFormContainer'

function CreateTransferBox(): JSX.Element {
  return (
    <Box style={{ marginTop: '2rem' }}>
      <Heading subtitle size={4} className='has-text-weight-light'>
        Make a transfer
      </Heading>
      <CreateTransferForm />
    </Box>
  )
}

export default CreateTransferBox
