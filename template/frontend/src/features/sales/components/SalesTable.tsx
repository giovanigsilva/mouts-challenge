import { Eye, Pencil, Trash2, XCircle } from 'lucide-react'
import { Link } from 'react-router-dom'

import { SaleStatusBadge } from '@/features/sales/components/SaleStatusBadge'
import type { Sale } from '@/features/sales/types/sale.types'
import { Button } from '@/shared/components/ui/button'
import { useLanguage } from '@/shared/i18n/use-language'
import { formatDateTime } from '@/shared/lib/dates'
import { formatMoney } from '@/shared/lib/money'

type SalesTableProps = {
  sales: Sale[]
  onCancel: (sale: Sale) => void
  onDelete: (sale: Sale) => void
}

export function SalesTable({ sales, onCancel, onDelete }: SalesTableProps) {
  const { t } = useLanguage()

  if (!sales.length) {
    return <div className="rounded-2xl border border-white/10 bg-white/5 p-8 text-center text-slate-400">{t('noSalesFound')}</div>
  }

  return (
    <div className="overflow-hidden rounded-2xl border border-white/10 bg-white/5">
      <div className="overflow-x-auto">
        <table className="w-full min-w-[960px] text-left text-sm">
          <thead className="bg-white/5 text-xs uppercase tracking-wide text-slate-400">
            <tr>
              <th className="px-4 py-3">{t('number')}</th>
              <th className="px-4 py-3">{t('customer')}</th>
              <th className="px-4 py-3">{t('branch')}</th>
              <th className="px-4 py-3">{t('date')}</th>
              <th className="px-4 py-3">{t('total')}</th>
              <th className="px-4 py-3">{t('status')}</th>
              <th className="px-4 py-3">{t('items')}</th>
              <th className="px-4 py-3 text-right">{t('actions')}</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/10">
            {sales.map((sale) => (
              <tr key={sale.id} className="text-slate-200">
                <td className="px-4 py-3 font-medium text-white">{sale.saleNumber}</td>
                <td className="px-4 py-3">{sale.customerName}</td>
                <td className="px-4 py-3">{sale.branchName}</td>
                <td className="px-4 py-3">{formatDateTime(sale.saleDate)}</td>
                <td className="px-4 py-3">{formatMoney(sale.totalAmount)}</td>
                <td className="px-4 py-3">
                  <SaleStatusBadge isCancelled={sale.isCancelled} />
                </td>
                <td className="px-4 py-3">{sale.items.length}</td>
                <td className="px-4 py-3">
                  <div className="flex justify-end gap-2">
                    <Button asChild variant="ghost" size="icon" title={t('viewDetails')}>
                      <Link to={`/sales/${sale.id}`}>
                        <Eye className="h-4 w-4" />
                      </Link>
                    </Button>
                    <Button asChild variant="ghost" size="icon" title={t('editAction')} disabled={sale.isCancelled}>
                      <Link to={`/sales/${sale.id}/edit`}>
                        <Pencil className="h-4 w-4" />
                      </Link>
                    </Button>
                    <Button variant="ghost" size="icon" title={t('cancelSaleAction')} disabled={sale.isCancelled} onClick={() => onCancel(sale)}>
                      <XCircle className="h-4 w-4" />
                    </Button>
                    <Button variant="ghost" size="icon" title={t('removeSaleAction')} onClick={() => onDelete(sale)}>
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
