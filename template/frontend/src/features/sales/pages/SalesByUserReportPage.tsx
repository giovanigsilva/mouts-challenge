import { BarChart3, RefreshCw } from 'lucide-react'
import { useState } from 'react'

import { useSalesByUserReport } from '@/features/sales/hooks/use-sales'
import type { SalesByUserReportItem } from '@/features/sales/types/sale.types'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { Button } from '@/shared/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import { useLanguage } from '@/shared/i18n/use-language'
import { formatDateTime } from '@/shared/lib/dates'
import { formatMoney } from '@/shared/lib/money'

export function SalesByUserReportPage() {
  const { t } = useLanguage()
  const [fromDate, setFromDate] = useState('')
  const [toDate, setToDate] = useState('')
  const query = useSalesByUserReport({
    fromDate: fromDate || undefined,
    toDate: toDate || undefined,
  })

  const items = query.data ?? []
  const totals = items.reduce(
    (accumulator, item) => ({
      totalSales: accumulator.totalSales + item.totalSales,
      activeSales: accumulator.activeSales + item.activeSales,
      cancelledSales: accumulator.cancelledSales + item.cancelledSales,
      totalSoldAmount: accumulator.totalSoldAmount + item.totalSoldAmount,
    }),
    { totalSales: 0, activeSales: 0, cancelledSales: 0, totalSoldAmount: 0 },
  )

  const apiError = query.error as NormalizedApiError | null

  return (
    <ContentContainer>
      <PageHeader
        title={t('salesByUserReport')}
        description={t('salesByUserReportDescription')}
        actions={
          <Button type="button" variant="secondary" onClick={() => void query.refetch()}>
            <RefreshCw className="h-4 w-4" />
            {t('update')}
          </Button>
        }
      />

      <div className="grid gap-4 md:grid-cols-4">
        <Metric label={t('salesDone')} value={totals.totalSales.toString()} />
        <Metric label={t('salesConfirmed')} value={totals.activeSales.toString()} />
        <Metric label={t('salesCancelled')} value={totals.cancelledSales.toString()} />
        <Metric label={t('totalSold')} value={formatMoney(totals.totalSoldAmount)} />
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{t('reportFilters')}</CardTitle>
        </CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2">
          <div className="space-y-2">
            <Label htmlFor="fromDate">{t('from')}</Label>
            <Input id="fromDate" type="date" value={fromDate} onChange={(event) => setFromDate(event.target.value)} />
          </div>
          <div className="space-y-2">
            <Label htmlFor="toDate">{t('to')}</Label>
            <Input id="toDate" type="date" value={toDate} onChange={(event) => setToDate(event.target.value)} />
          </div>
        </CardContent>
      </Card>

      {query.isLoading ? <p className="text-sm text-slate-400">{t('loading')}</p> : null}
      {apiError ? <p className="rounded-xl border border-rose-300/20 bg-rose-300/10 p-4 text-sm text-rose-100">{apiError.message}</p> : null}

      <div className="overflow-hidden rounded-2xl border border-white/10 bg-white/5">
        <table className="w-full min-w-[900px] text-left text-sm">
          <thead className="bg-white/5 text-xs uppercase tracking-wide text-slate-400">
            <tr>
              <th className="px-4 py-3">{t('employee')}</th>
              <th className="px-4 py-3">{t('email')}</th>
              <th className="px-4 py-3">{t('profile')}</th>
              <th className="px-4 py-3">{t('salesDone')}</th>
              <th className="px-4 py-3">{t('salesConfirmed')}</th>
              <th className="px-4 py-3">{t('salesCancelled')}</th>
              <th className="px-4 py-3">{t('totalSold')}</th>
              <th className="px-4 py-3">{t('period')}</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/10">
            {items.map((item) => (
              <ReportRow key={item.userId} item={item} />
            ))}
            {!query.isLoading && items.length === 0 ? (
              <tr>
                <td colSpan={8} className="px-4 py-8 text-center text-slate-400">
                  {t('noReportData')}
                </td>
              </tr>
            ) : null}
          </tbody>
        </table>
      </div>
    </ContentContainer>
  )
}

function Metric({ label, value }: { label: string; value: string }) {
  return (
    <Card>
      <CardContent className="flex items-center gap-4 p-5">
        <div className="rounded-xl bg-cyan-300/10 p-3 text-cyan-200">
          <BarChart3 className="h-5 w-5" />
        </div>
        <div>
          <p className="text-sm text-slate-400">{label}</p>
          <p className="mt-1 text-2xl font-semibold text-white">{value}</p>
        </div>
      </CardContent>
    </Card>
  )
}

function ReportRow({ item }: { item: SalesByUserReportItem }) {
  const { t } = useLanguage()
  const roleLabel = String(item.role) === '3' || item.role === 'Admin' ? t('administrator') : String(item.role)
  const firstDate = item.firstSaleDate ? formatDateTime(item.firstSaleDate) : '-'
  const lastDate = item.lastSaleDate ? formatDateTime(item.lastSaleDate) : '-'

  return (
    <tr>
      <td className="px-4 py-3 font-medium text-white">{item.username}</td>
      <td className="px-4 py-3">{item.email}</td>
      <td className="px-4 py-3">{roleLabel}</td>
      <td className="px-4 py-3">{item.totalSales}</td>
      <td className="px-4 py-3">{item.activeSales}</td>
      <td className="px-4 py-3">{item.cancelledSales}</td>
      <td className="px-4 py-3">{formatMoney(item.totalSoldAmount)}</td>
      <td className="px-4 py-3">{firstDate} - {lastDate}</td>
    </tr>
  )
}
