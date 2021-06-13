import { Config } from '../types'

function getConfig(): Config {
  const { REACT_APP_BACKEND_URL } = process.env

  if (!REACT_APP_BACKEND_URL) {
    throw new Error('Missing environment variable REACT_APP_BACKEND_URL')
  }

  return {
    backendUrl: REACT_APP_BACKEND_URL,
  }
}

export { getConfig }
