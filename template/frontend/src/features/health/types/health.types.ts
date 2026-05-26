export type HealthCheckItem = {
  name: string
  status: string
  description?: string | null
  errorMessage?: string | null
  hostEnvironment?: string
}

export type HealthResponse = {
  status: string
  healthChecks?: HealthCheckItem[]
  [key: string]: unknown
}

export type HealthEndpointKey = 'live' | 'ready' | 'logging' | 'cache' | 'metrics'

export type HealthEndpointResult = {
  key: HealthEndpointKey
  label: string
  status: 'Healthy' | 'Unhealthy' | 'Unavailable'
  elapsedMs: number
  correlationId?: string
  data?: HealthResponse
}
