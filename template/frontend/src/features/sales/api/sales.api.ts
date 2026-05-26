import { apiClient, unwrapApiData } from '@/shared/api/api-client'
import type { ApiEnvelope } from '@/shared/types/api-response'
import type { Sale, SaleRequest, SalesFilters, SalesPage } from '@/features/sales/types/sale.types'

type BackendPaginatedResponse<T> = ApiEnvelope<T[]> & {
  currentPage: number
  totalPages: number
  totalCount: number
}

export async function listSales(filters: SalesFilters) {
  const response = await apiClient.get<BackendPaginatedResponse<Sale>>('/api/sales', {
    params: filters,
  })

  return {
    items: response.data.data ?? [],
    currentPage: response.data.currentPage,
    totalPages: response.data.totalPages,
    totalCount: response.data.totalCount,
  } satisfies SalesPage
}

export async function getSale(id: string) {
  const response = await apiClient.get<ApiEnvelope<Sale>>(`/api/sales/${id}`)

  return unwrapApiData(response.data)
}

export async function createSale(request: SaleRequest) {
  const response = await apiClient.post<ApiEnvelope<Sale>>('/api/sales', request)

  return unwrapApiData(response.data)
}

export async function updateSale(id: string, request: SaleRequest) {
  const response = await apiClient.put<ApiEnvelope<Sale>>(`/api/sales/${id}`, request)

  return unwrapApiData(response.data)
}

export async function deleteSale(id: string) {
  await apiClient.delete<ApiEnvelope>(`/api/sales/${id}`)
}

export async function cancelSale(id: string) {
  const response = await apiClient.patch<ApiEnvelope<Sale>>(`/api/sales/${id}/cancel`)

  return unwrapApiData(response.data)
}

export async function cancelSaleItem(id: string, itemId: string) {
  const response = await apiClient.patch<ApiEnvelope<Sale>>(`/api/sales/${id}/items/${itemId}/cancel`)

  return unwrapApiData(response.data)
}
