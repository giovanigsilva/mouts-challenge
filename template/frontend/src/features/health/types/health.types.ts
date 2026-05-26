export type HealthCheckItem = {
  name: string
  status: string
  description?: string | null
  errorMessage?: string | null
  hostEnvironment?: string
}

export type EnterpriseServiceHealth = {
  name: string
  status: string
  target?: string
}

export type HealthResponse = {
  status: string
  serviceName?: string
  services?: EnterpriseServiceHealth[]
  healthChecks?: HealthCheckItem[]
  [key: string]: unknown
}

export type HealthEndpointKey = 'live' | 'ready' | 'logging' | 'cache' | 'metrics' | 'enterprise'

export type HealthEndpointResult = {
  key: HealthEndpointKey
  label: string
  path: string
  status: 'Healthy' | 'Unhealthy' | 'Unavailable'
  elapsedMs: number
  correlationId?: string
  data?: HealthResponse
}
