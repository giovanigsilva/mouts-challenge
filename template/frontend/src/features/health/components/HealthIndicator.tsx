import { Badge } from '@/shared/components/ui/badge'
import { useLanguage } from '@/shared/i18n/use-language'

type HealthIndicatorProps = {
  status: 'Healthy' | 'Unhealthy' | 'Unavailable'
}

export function HealthIndicator({ status }: HealthIndicatorProps) {
  const { t } = useLanguage()

  if (status === 'Healthy') {
    return <Badge variant="success">{t('healthy')}</Badge>
  }

  if (status === 'Unavailable') {
    return <Badge variant="danger">{t('unavailable')}</Badge>
  }

  return <Badge variant="warning">{t('degraded')}</Badge>
}
