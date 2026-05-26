import { Badge } from '@/shared/components/ui/badge'

type SaleStatusBadgeProps = {
  isCancelled: boolean
}

export function SaleStatusBadge({ isCancelled }: SaleStatusBadgeProps) {
  return <Badge variant={isCancelled ? 'danger' : 'success'}>{isCancelled ? 'Cancelada' : 'Ativa'}</Badge>
}
