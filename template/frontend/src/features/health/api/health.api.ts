import { apiClient } from '@/shared/api/api-client'
import type { HealthEndpointKey, HealthEndpointResult, HealthResponse } from '@/features/health/types/health.types'

const endpoints: Record<HealthEndpointKey, { path: string; label: string }> = {
  live: { path: '/health/live', label: 'Liveness' },
  ready: { path: '/health/ready', label: 'Readiness' },
  logging: { path: '/health/logging', label: 'Logging' },
  cache: { path: '/health/cache', label: 'Cache' },
  metrics: { path: '/health/metrics', label: 'Metrics' },
}

export async function getHealth(key: HealthEndpointKey): Promise<HealthEndpointResult> {
  const endpoint = endpoints[key]
  const startedAt = performance.now()

  try {
    const response = await apiClient.get<HealthResponse>(endpoint.path)

    return {
      key,
      label: endpoint.label,
      status: response.data.status === 'Healthy' ? 'Healthy' : 'Unhealthy',
      elapsedMs: Math.round(performance.now() - startedAt),
      correlationId: response.headers['x-correlation-id'],
      data: response.data,
    }
  } catch {
    return {
      key,
      label: endpoint.label,
      status: 'Unavailable',
      elapsedMs: Math.round(performance.now() - startedAt),
    }
  }
}

export const healthKeys = Object.keys(endpoints) as HealthEndpointKey[]
