import { Plus } from 'lucide-react'
import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { toast } from 'sonner'

import { SalesFilters } from '@/features/sales/components/SalesFilters'
import { SalesTable } from '@/features/sales/components/SalesTable'
import { useCancelSale, useDeleteSale, useSalesList } from '@/features/sales/hooks/use-sales'
import type { Sale, SalesFilters as SalesFiltersType } from '@/features/sales/types/sale.types'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { Button } from '@/shared/components/ui/button'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { useConfirmDialog } from '@/shared/hooks/use-confirm-dialog'
import { useLanguage } from '@/shared/i18n/use-language'

const initialFilters: SalesFiltersType = {
  page: 1,
  pageSize: 10,
}

export function SalesListPage() {
  const navigate = useNavigate()
  const [filters, setFilters] = useState(initialFilters)
  const salesQuery = useSalesList(filters)
  const cancelMutation = useCancelSale()
  const deleteMutation = useDeleteSale()
  const confirmDialog = useConfirmDialog()
  const { t } = useLanguage()

  function handleCancel(sale: Sale) {
    if (!confirmDialog.confirm(t('confirmCancelSale', { saleNumber: sale.saleNumber }))) {
      return
    }

    cancelMutation.mutate(sale.id, {
      onSuccess: () => toast.success(t('saleCancelled')),
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  function handleDelete(sale: Sale) {
    if (!confirmDialog.confirm(t('confirmDeleteSale', { saleNumber: sale.saleNumber }))) {
      return
    }

    deleteMutation.mutate(sale.id, {
      onSuccess: () => {
        toast.success(t('saleRemoved'))
        navigate('/sales')
      },
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  return (
    <ContentContainer>
      <PageHeader
        title={t('sales')}
        description={t('salesListDescription')}
        actions={
          <Button asChild>
            <Link to="/sales/new">
              <Plus className="h-4 w-4" />
              {t('newSale')}
            </Link>
          </Button>
        }
      />

      <div className="space-y-5">
        <SalesFilters filters={filters} onChange={setFilters} />

        {salesQuery.isLoading ? <div className="rounded-2xl border border-white/10 bg-white/5 p-8 text-slate-400">{t('loadingSales')}</div> : null}
        {salesQuery.error ? (
          <div className="rounded-2xl border border-rose-300/20 bg-rose-300/10 p-5 text-rose-100">
            {(salesQuery.error as unknown as NormalizedApiError).message}
          </div>
        ) : null}
        {salesQuery.data ? <SalesTable sales={salesQuery.data.items} onCancel={handleCancel} onDelete={handleDelete} /> : null}

        {salesQuery.data ? (
          <div className="flex items-center justify-between text-sm text-slate-400">
            <span>
              {t('page')} {salesQuery.data.currentPage} {t('of')} {salesQuery.data.totalPages || 1}. {t('total')}: {salesQuery.data.totalCount}
            </span>
            <div className="flex gap-2">
              <Button
                variant="secondary"
                size="sm"
                disabled={filters.page <= 1}
                onClick={() => setFilters((current) => ({ ...current, page: current.page - 1 }))}
              >
                {t('previous')}
              </Button>
              <Button
                variant="secondary"
                size="sm"
                disabled={filters.page >= salesQuery.data.totalPages}
                onClick={() => setFilters((current) => ({ ...current, page: current.page + 1 }))}
              >
                {t('next')}
              </Button>
            </div>
          </div>
        ) : null}
      </div>
    </ContentContainer>
  )
}
