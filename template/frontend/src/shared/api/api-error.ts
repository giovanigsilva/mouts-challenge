import axios, { type AxiosError } from 'axios'

import type { ApiEnvelope } from '@/shared/types/api-response'

export type NormalizedApiError = {
  status?: number
  message: string
  correlationId?: string
  errors: string[]
}

const defaultMessages: Record<number, string> = {
  400: 'A requisição possui dados inválidos.',
  401: 'Sessão expirada ou token inválido.',
  403: 'Você não possui permissão para executar esta operação.',
  404: 'Recurso não encontrado.',
  409: 'Conflito ou violação de regra de negócio.',
  429: 'Muitas requisições em um curto período. Tente novamente mais tarde.',
  499: 'A requisição foi cancelada.',
  500: 'Ocorreu um erro interno na API.',
}

export function normalizeApiError(error: unknown): NormalizedApiError {
  if (!axios.isAxiosError(error)) {
    return {
      message: 'Ocorreu um erro inesperado.',
      errors: [],
    }
  }

  const axiosError = error as AxiosError<ApiEnvelope>
  const status = axiosError.response?.status
  const payload = axiosError.response?.data
  const errors = normalizeErrors(payload?.errors)

  return {
    status,
    message: payload?.message || (status ? defaultMessages[status] : undefined) || axiosError.message || 'Falha ao comunicar com a API.',
    correlationId: payload?.correlationId || axiosError.response?.headers['x-correlation-id'],
    errors,
  }
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
