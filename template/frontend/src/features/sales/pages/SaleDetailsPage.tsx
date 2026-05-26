import { Pencil, Trash2, XCircle } from 'lucide-react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { toast } from 'sonner'

import { SaleDetailsPanel } from '@/features/sales/components/SaleDetailsPanel'
import { useCancelSale, useCancelSaleItem, useDeleteSale, useSale } from '@/features/sales/hooks/use-sales'
import type { SaleItem } from '@/features/sales/types/sale.types'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { Button } from '@/shared/components/ui/button'
import { useConfirmDialog } from '@/shared/hooks/use-confirm-dialog'
import { useLanguage } from '@/shared/i18n/use-language'

export function SaleDetailsPage() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const saleQuery = useSale(id)
  const cancelMutation = useCancelSale()
  const cancelItemMutation = useCancelSaleItem()
  const deleteMutation = useDeleteSale()
  const confirmDialog = useConfirmDialog()
  const { t } = useLanguage()

  function handleCancelSale() {
    if (!saleQuery.data || !confirmDialog.confirm(t('confirmCancelSale', { saleNumber: saleQuery.data.saleNumber }))) {
      return
    }

    cancelMutation.mutate(saleQuery.data.id, {
      onSuccess: () => toast.success(t('saleCancelled')),
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  function handleCancelItem(item: SaleItem) {
    if (!saleQuery.data) {
      return
    }

    if (saleQuery.data.items.length === 1) {
      if (!confirmDialog.confirm(t('confirmCancelLastItem', { productName: item.productName, saleNumber: saleQuery.data.saleNumber }))) {
        return
      }

      cancelMutation.mutate(saleQuery.data.id, {
        onSuccess: () => toast.success(t('saleCancelled')),
        onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
      })

      return
    }

    if (!confirmDialog.confirm(t('confirmCancelItem', { productName: item.productName }))) {
      return
    }

    cancelItemMutation.mutate(
      { saleId: saleQuery.data.id, itemId: item.id },
      {
        onSuccess: () => toast.success(t('itemCancelled')),
        onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
      },
    )
  }

  function handleDelete() {
    if (!saleQuery.data || !confirmDialog.confirm(t('confirmDeleteSale', { saleNumber: saleQuery.data.saleNumber }))) {
      return
    }

    deleteMutation.mutate(saleQuery.data.id, {
      onSuccess: () => {
        toast.success(t('saleRemoved'))
        navigate('/sales')
      },
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  if (saleQuery.isLoading) {
    return <ContentContainer>{t('loadingSale')}</ContentContainer>
  }

  if (saleQuery.error) {
    return <ContentContainer>{(saleQuery.error as unknown as NormalizedApiError).message}</ContentContainer>
  }

  if (!saleQuery.data) {
    return <ContentContainer>{t('saleNotFound')}</ContentContainer>
  }

  const sale = saleQuery.data

  return (
    <ContentContainer>
      <PageHeader
        title={`${t('sale')} ${sale.saleNumber}`}
        description={t('saleDetailsDescription')}
        actions={
          <>
            <Button asChild variant="secondary" disabled={sale.isCancelled}>
              <Link to={`/sales/${sale.id}/edit`}>
                <Pencil className="h-4 w-4" />
                {t('edit')}
              </Link>
            </Button>
            <Button variant="secondary" disabled={sale.isCancelled} onClick={handleCancelSale}>
              <XCircle className="h-4 w-4" />
              {t('cancel')}
            </Button>
            <Button variant="destructive" onClick={handleDelete}>
              <Trash2 className="h-4 w-4" />
              {t('remove')}
            </Button>
          </>
        }
      />
      <SaleDetailsPanel sale={sale} onCancelItem={handleCancelItem} />
    </ContentContainer>
  )
}
