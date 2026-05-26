import { z } from 'zod'

import { translations, type TranslationKey } from '@/shared/i18n/translations'

type Translate = (key: TranslationKey) => string

export function createUserSchema(t: Translate) {
  return z.object({
    username: z.string().min(1, t('validationNameRequired')),
    email: z.string().email(t('validationEmailValid')),
    phone: z.string().min(8, t('validationPhoneRequired')),
    password: z.string().min(6, t('validationPasswordMin')),
    status: z.number().min(1, t('validationStatusRequired')),
    role: z.number().min(1, t('validationRoleRequired')),
  })
}

export const createUserSchemaPtBr = createUserSchema((key) => translations['pt-BR'][key])

export type CreateUserFormValues = z.infer<ReturnType<typeof createUserSchema>>
