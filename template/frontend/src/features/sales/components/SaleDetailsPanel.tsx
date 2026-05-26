import { XCircle } from 'lucide-react'

import { SaleStatusBadge } from '@/features/sales/components/SaleStatusBadge'
import { SaleTotalsCard } from '@/features/sales/components/SaleTotalsCard'
import type { Sale, SaleItem } from '@/features/sales/types/sale.types'
import { Badge } from '@/shared/components/ui/badge'
import { Button } from '@/shared/components/ui/button'
import { useLanguage } from '@/shared/i18n/use-language'
import { formatDateTime } from '@/shared/lib/dates'
import { formatMoney } from '@/shared/lib/money'

type SaleDetailsPanelProps = {
  sale: Sale
  onCancelItem: (item: SaleItem) => void
}

export function SaleDetailsPanel({ sale, onCancelItem }: SaleDetailsPanelProps) {
  const { t } = useLanguage()

  return (
    <div className="space-y-6">
      <div className="grid gap-4 lg:grid-cols-4">
        <SaleTotalsCard totalAmount={sale.totalAmount} itemsCount={sale.items.length} />
        <InfoCard label={t('number')} value={sale.saleNumber} />
        <InfoCard label={t('date')} value={formatDateTime(sale.saleDate)} />
        <div className="rounded-2xl border border-white/10 bg-white/5 p-5">
          <p className="text-sm text-slate-400">{t('status')}</p>
          <div className="mt-2">
            <SaleStatusBadge isCancelled={sale.isCancelled} />
          </div>
        </div>
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <InfoCard label={`${t('customer')} / ${t('customerExternalId')}`} value={`${sale.customerName} (${sale.customerExternalId})`} />
        <InfoCard label={`${t('branch')} / ${t('branchExternalId')}`} value={`${sale.branchName} (${sale.branchExternalId})`} />
      </div>

      <div className="overflow-hidden rounded-2xl border border-white/10 bg-white/5">
        <table className="w-full min-w-[900px] text-left text-sm">
          <thead className="bg-white/5 text-xs uppercase tracking-wide text-slate-400">
            <tr>
              <th className="px-4 py-3">{t('product')}</th>
              <th className="px-4 py-3">{t('quantity')}</th>
              <th className="px-4 py-3">{t('unitPrice')}</th>
              <th className="px-4 py-3">{t('discount')}</th>
              <th className="px-4 py-3">{t('total')}</th>
              <th className="px-4 py-3">{t('status')}</th>
              <th className="px-4 py-3 text-right">{t('actions')}</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/10">
            {sale.items.map((item) => (
              <tr key={item.id}>
                <td className="px-4 py-3 text-white">{item.productName}</td>
                <td className="px-4 py-3">{item.quantity}</td>
                <td className="px-4 py-3">{formatMoney(item.unitPrice)}</td>
                <td className="px-4 py-3">
                  {item.discountPercentage}% ({formatMoney(item.discountAmount)})
                </td>
                <td className="px-4 py-3">{formatMoney(item.totalAmount)}</td>
                <td className="px-4 py-3">
                  <Badge variant={item.isCancelled ? 'danger' : 'success'}>{item.isCancelled ? t('itemStatusCancelled') : t('itemStatusActive')}</Badge>
                </td>
                <td className="px-4 py-3 text-right">
                  <Button variant="ghost" size="sm" disabled={sale.isCancelled || item.isCancelled} onClick={() => onCancelItem(item)}>
                    <XCircle className="h-4 w-4" />
                    {t('cancelItem')}
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

function InfoCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-2xl border border-white/10 bg-white/5 p-5">
      <p className="text-sm text-slate-400">{label}</p>
      <p className="mt-2 break-all text-sm font-medium text-white">{value}</p>
    </div>
  )
}
