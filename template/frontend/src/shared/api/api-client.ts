import axios from 'axios'
import { toast } from 'sonner'

import { normalizeApiError } from '@/shared/api/api-error'
import { authTokenStorage } from '@/shared/api/auth-token-storage'
import { createCorrelationId } from '@/shared/api/correlation-id'
import { env } from '@/shared/config/env'
import type { ApiEnvelope } from '@/shared/types/api-response'

export const apiClient = axios.create({
  baseURL: env.apiBaseUrl,
  timeout: 20_000,
  headers: {
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use((config) => {
  const token = authTokenStorage.getToken()

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  config.headers['X-Correlation-Id'] = createCorrelationId()

  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const normalized = normalizeApiError(error)

    if (normalized.status === 401) {
      authTokenStorage.clearToken()
      toast.error('Sessão expirada ou token inválido.')

      if (window.location.pathname !== '/login') {
        window.location.assign('/login')
      }
    }

    return Promise.reject(normalized)
  },
)

export function unwrapApiData<T>(envelope: ApiEnvelope<T>): T {
  if (!envelope.success) {
    throw new Error(envelope.message || 'A API retornou uma resposta sem sucesso.')
  }

  return envelope.data as T
}
