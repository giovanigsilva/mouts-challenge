import { recaptchaConfig } from '@/shared/security/recaptcha/recaptcha.config'

export async function executeRecaptcha(action: string): Promise<string | null> {
  if (!recaptchaConfig.enabled) {
    return null
  }

  if (recaptchaConfig.provider !== 'simulated') {
    throw new Error('Provider de reCAPTCHA nao suportado neste frontend local.')
  }

  const timestamp = Math.floor(Date.now() / 1000)
  const nonce = crypto.randomUUID()

  return `simulated:${action}:${timestamp}:${nonce}`
}
