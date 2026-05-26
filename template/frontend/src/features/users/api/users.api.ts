import { apiClient, unwrapApiData } from '@/shared/api/api-client'
import type { ApiEnvelope } from '@/shared/types/api-response'
import type { CreateUserRequest, UserResponse } from '@/features/users/types/user.types'

export async function createUser(request: CreateUserRequest) {
  const response = await apiClient.post<ApiEnvelope<UserResponse>>('/api/Users', request)

  return unwrapApiData(response.data)
}
