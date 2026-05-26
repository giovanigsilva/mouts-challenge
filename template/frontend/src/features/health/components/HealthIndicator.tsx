import { Badge } from '@/shared/components/ui/badge'

type HealthIndicatorProps = {
  status: 'Healthy' | 'Unhealthy' | 'Unavailable'
}

export function HealthIndicator({ status }: HealthIndicatorProps) {
  if (status === 'Healthy') {
    return <Badge variant="success">Saudavel</Badge>
  }

  if (status === 'Unavailable') {
    return <Badge variant="danger">Indisponivel</Badge>
  }

  return <Badge variant="warning">Degradado</Badge>
}
