import { Badge } from '@/shared/components/ui/badge'
import { useLanguage } from '@/shared/i18n/use-language'

type SaleStatusBadgeProps = {
  isCancelled: boolean
}

export function SaleStatusBadge({ isCancelled }: SaleStatusBadgeProps) {
  const { t } = useLanguage()

  return <Badge variant={isCancelled ? 'danger' : 'success'}>{isCancelled ? t('saleStatusCancelled') : t('saleStatusActive')}</Badge>
}
