const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080'
const appName = import.meta.env.VITE_APP_NAME ?? 'DeveloperStore Frontend'
const appEnvironment = import.meta.env.VITE_APP_ENV ?? 'development'
const recaptchaEnabled = import.meta.env.VITE_RECAPTCHA_ENABLED === 'true'
const recaptchaProvider = import.meta.env.VITE_RECAPTCHA_PROVIDER ?? 'simulated'
const recaptchaLoginAction = import.meta.env.VITE_RECAPTCHA_LOGIN_ACTION ?? 'login'
const recaptchaCreateUserAction = import.meta.env.VITE_RECAPTCHA_CREATE_USER_ACTION ?? 'create_user'

export const env = {
  apiBaseUrl: apiBaseUrl.replace(/\/$/, ''),
  appName,
  appEnvironment,
  recaptcha: {
    enabled: recaptchaEnabled,
    provider: recaptchaProvider,
    loginAction: recaptchaLoginAction,
    createUserAction: recaptchaCreateUserAction,
  },
}
