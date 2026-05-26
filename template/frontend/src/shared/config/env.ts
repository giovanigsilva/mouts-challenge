const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080'
const appName = import.meta.env.VITE_APP_NAME ?? 'DeveloperStore Frontend'
const appEnvironment = import.meta.env.VITE_APP_ENV ?? 'development'

export const env = {
  apiBaseUrl: apiBaseUrl.replace(/\/$/, ''),
  appName,
  appEnvironment,
}
