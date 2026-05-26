import { z } from 'zod'

import { translations, type TranslationKey } from '@/shared/i18n/translations'

type Translate = (key: TranslationKey) => string

export function createLoginSchema(t: Translate) {
  return z.object({
    email: z.string().min(1, t('validationEmailRequired')).email(t('validationEmailValid')),
    password: z.string().min(1, t('validationPasswordRequired')),
  })
}

export const loginSchema = createLoginSchema((key) => translations['pt-BR'][key])

export type LoginFormValues = z.infer<ReturnType<typeof createLoginSchema>>
