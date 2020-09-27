function getBackendUrl(): string {
  const { REACT_APP_BACKEND_URL } = process.env

  if (!REACT_APP_BACKEND_URL) {
    throw new Error('Missing environment variable REACT_APP_BACKEND_URL')
  }

  return REACT_APP_BACKEND_URL
}

export { getBackendUrl }
