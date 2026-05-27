import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'

import { cancelSale, cancelSaleItem, createSale, deleteSale, getSale, getSalesByUserReport, listSales, updateSale } from '@/features/sales/api/sales.api'
import type { SaleRequest, SalesFilters } from '@/features/sales/types/sale.types'

export const salesQueryKeys = {
  all: ['sales'] as const,
  list: (filters: SalesFilters) => ['sales', 'list', filters] as const,
  detail: (id: string) => ['sales', 'detail', id] as const,
  reportByUser: (filters: { fromDate?: string; toDate?: string }) => ['sales', 'reports', 'by-user', filters] as const,
}

export function useSalesList(filters: SalesFilters) {
  return useQuery({
    queryKey: salesQueryKeys.list(filters),
    queryFn: () => listSales(filters),
  })
}

export function useSale(id: string) {
  return useQuery({
    queryKey: salesQueryKeys.detail(id),
    queryFn: () => getSale(id),
    enabled: Boolean(id),
  })
}

export function useSalesByUserReport(filters: { fromDate?: string; toDate?: string }) {
  return useQuery({
    queryKey: salesQueryKeys.reportByUser(filters),
    queryFn: () => getSalesByUserReport(filters),
  })
}

export function useCreateSale() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: createSale,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: salesQueryKeys.all }),
  })
}

export function useUpdateSale(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: SaleRequest) => updateSale(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: salesQueryKeys.all })
      queryClient.invalidateQueries({ queryKey: salesQueryKeys.detail(id) })
    },
  })
}

export function useDeleteSale() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: deleteSale,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: salesQueryKeys.all }),
  })
}

export function useCancelSale() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: cancelSale,
    onSuccess: (sale) => {
      queryClient.invalidateQueries({ queryKey: salesQueryKeys.all })
      queryClient.invalidateQueries({ queryKey: salesQueryKeys.detail(sale.id) })
    },
  })
}

export function useCancelSaleItem() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ saleId, itemId }: { saleId: string; itemId: string }) => cancelSaleItem(saleId, itemId),
    onSuccess: (sale) => {
      queryClient.invalidateQueries({ queryKey: salesQueryKeys.all })
      queryClient.invalidateQueries({ queryKey: salesQueryKeys.detail(sale.id) })
    },
  })
}
