import axios, { type AxiosError } from 'axios'

import type { ApiEnvelope } from '@/shared/types/api-response'

export type NormalizedApiError = {
  status?: number
  message: string
  correlationId?: string
  errors: string[]
}

const defaultMessages: Record<number, string> = {
  400: 'A requisicao possui dados invalidos.',
  401: 'Sessao expirada ou token invalido.',
  403: 'Voce nao possui permissao para executar esta operacao.',
  404: 'Recurso nao encontrado.',
  409: 'Conflito ou violacao de regra de negocio.',
  429: 'Muitas requisicoes em um curto periodo. Tente novamente mais tarde.',
  499: 'A requisicao foi cancelada.',
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
