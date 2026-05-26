import { apiClient, unwrapApiData } from '@/shared/api/api-client'
import type { ApiEnvelope } from '@/shared/types/api-response'
import type { LoginRequest, LoginResponse } from '@/features/auth/types/auth.types'

export async function login(request: LoginRequest) {
  const response = await apiClient.post<ApiEnvelope<LoginResponse>>('/api/Auth', request)

  return unwrapApiData(response.data)
}
