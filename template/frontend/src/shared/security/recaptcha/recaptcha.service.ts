import { recaptchaConfig } from '@/shared/security/recaptcha/recaptcha.config'
import { translations, type Language } from '@/shared/i18n/translations'

export async function executeRecaptcha(action: string): Promise<string | null> {
  if (!recaptchaConfig.enabled) {
    return null
  }

  if (recaptchaConfig.provider !== 'simulated') {
    throw new Error(getUnsupportedProviderMessage())
  }

  const timestamp = Math.floor(Date.now() / 1000)
  const nonce = crypto.randomUUID()

  return `simulated:${action}:${timestamp}:${nonce}`
}

function getUnsupportedProviderMessage() {
  const stored = localStorage.getItem('developerstore.language')
  const language: Language = stored === 'en' || stored === 'es' || stored === 'pt-BR' ? stored : 'pt-BR'

  return translations[language].unsupportedRecaptchaProvider
}
