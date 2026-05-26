import axios, { type AxiosError } from 'axios'

import type { ApiEnvelope } from '@/shared/types/api-response'
import { translations, type Language, type TranslationKey } from '@/shared/i18n/translations'

export type NormalizedApiError = {
  status?: number
  message: string
  correlationId?: string
  errors: string[]
}

const defaultMessageKeys: Record<number, TranslationKey> = {
  400: 'error400',
  401: 'error401',
  403: 'error403',
  404: 'error404',
  409: 'error409',
  429: 'error429',
  499: 'error499',
  500: 'error500',
}

export function normalizeApiError(error: unknown): NormalizedApiError {
  if (!axios.isAxiosError(error)) {
    return {
      message: translate('unexpectedError'),
      errors: [],
    }
  }

  const axiosError = error as AxiosError<ApiEnvelope>
  const status = axiosError.response?.status
  const payload = axiosError.response?.data
  const errors = normalizeErrors(payload?.errors)

  return {
    status,
    message: payload?.message || (status && defaultMessageKeys[status] ? translate(defaultMessageKeys[status]) : undefined) || axiosError.message || translate('apiCommunicationFailure'),
    correlationId: payload?.correlationId || axiosError.response?.headers['x-correlation-id'],
    errors,
  }
}

function translate(key: TranslationKey) {
  const stored = localStorage.getItem('developerstore.language')
  const language: Language = stored === 'en' || stored === 'es' || stored === 'pt-BR' ? stored : 'pt-BR'

  return translations[language][key]
}

function normalizeErrors(errors: ApiEnvelope['errors']): string[] {
  if (!errors) {
    return []
  }

  if (Array.isArray(errors)) {
    return errors.map((error) => (typeof error === 'string' ? error : error.message))
  }

  return Object.entries(errors).flatMap(([field, messages]) => messages.map((message) => `${field}: ${message}`))
}
