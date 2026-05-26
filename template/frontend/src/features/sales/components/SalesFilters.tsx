import type { ChangeEvent } from 'react'

import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import type { SalesFilters as SalesFiltersType } from '@/features/sales/types/sale.types'
import { useLanguage } from '@/shared/i18n/use-language'

type SalesFiltersProps = {
  filters: SalesFiltersType
  onChange: (filters: SalesFiltersType) => void
}

export function SalesFilters({ filters, onChange }: SalesFiltersProps) {
  const { t } = useLanguage()

  function updateField(event: ChangeEvent<HTMLInputElement | HTMLSelectElement>) {
    const { name, value } = event.target
    const nextValue = name === 'isCancelled' ? parseBoolean(value) : value || undefined

    onChange({ ...filters, page: 1, [name]: nextValue })
  }

  function clearFilters() {
    onChange({ page: 1, pageSize: filters.pageSize })
  }

  return (
    <div className="grid gap-4 rounded-2xl border border-white/10 bg-white/5 p-4 md:grid-cols-3 xl:grid-cols-6">
      <div className="space-y-2">
        <Label htmlFor="saleNumber">{t('number')}</Label>
        <Input id="saleNumber" name="saleNumber" value={filters.saleNumber ?? ''} onChange={updateField} />
      </div>
      <div className="space-y-2">
        <Label htmlFor="customerId">{t('customerExternalId')}</Label>
        <Input id="customerId" name="customerId" value={filters.customerId ?? ''} onChange={updateField} />
      </div>
      <div className="space-y-2">
        <Label htmlFor="branchId">{t('branchExternalId')}</Label>
        <Input id="branchId" name="branchId" value={filters.branchId ?? ''} onChange={updateField} />
      </div>
      <div className="space-y-2">
        <Label htmlFor="isCancelled">{t('status')}</Label>
        <select
          id="isCancelled"
          name="isCancelled"
          value={filters.isCancelled === undefined ? '' : String(filters.isCancelled)}
          onChange={updateField}
          className="h-11 w-full rounded-xl border border-white/10 bg-white/10 px-3 text-sm text-slate-100 outline-none"
        >
          <option value="">{t('all')}</option>
          <option value="false">{t('activePlural')}</option>
          <option value="true">{t('cancelledPlural')}</option>
        </select>
      </div>
      <div className="space-y-2">
        <Label htmlFor="fromDate">{t('from')}</Label>
        <Input id="fromDate" name="fromDate" type="date" value={filters.fromDate ?? ''} onChange={updateField} />
      </div>
      <div className="flex items-end">
        <Button type="button" variant="secondary" className="w-full" onClick={clearFilters}>
          {t('clear')}
        </Button>
      </div>
    </div>
  )
}

function parseBoolean(value: string) {
  if (value === 'true') {
    return true
  }

  if (value === 'false') {
    return false
  }

  return undefined
}
