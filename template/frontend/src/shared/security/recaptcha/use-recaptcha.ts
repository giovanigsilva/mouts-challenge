import { executeRecaptcha } from '@/shared/security/recaptcha/recaptcha.service'

export function useRecaptcha() {
  return { executeRecaptcha }
}
