import { getBackendUrl } from '../environment'

function mkEndpointUrl(endpoint: string): string {
  return `${getBackendUrl()}/${endpoint}`
}

export { mkEndpointUrl }
