import { getConfig } from './config'

function mkBackendUrl(endpoint: string): URL {
  const { backendUrl } = getConfig()
  return new URL(`${backendUrl}/${endpoint}`)
}

export { mkBackendUrl }
