import { useQueries } from '@tanstack/react-query'

import { getHealth, healthKeys } from '@/features/health/api/health.api'

export function useHealthStatus() {
  return useQueries({
    queries: healthKeys.map((key) => ({
      queryKey: ['health', key],
      queryFn: () => getHealth(key),
      staleTime: 15_000,
      retry: 1,
    })),
  })
}
