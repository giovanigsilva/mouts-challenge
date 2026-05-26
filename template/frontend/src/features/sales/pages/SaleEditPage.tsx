import { useNavigate, useParams } from 'react-router-dom'
import { toast } from 'sonner'

import { SaleForm } from '@/features/sales/components/SaleForm'
import { useSale, useUpdateSale } from '@/features/sales/hooks/use-sales'
import type { SaleRequest } from '@/features/sales/types/sale.types'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { useLanguage } from '@/shared/i18n/use-language'

export function SaleEditPage() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const saleQuery = useSale(id)
  const mutation = useUpdateSale(id)
  const { t } = useLanguage()

  function handleSubmit(request: SaleRequest) {
    mutation.mutate(request, {
      onSuccess: (sale) => {
        toast.success(t('saleUpdated'))
        navigate(`/sales/${sale.id}`)
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

  if (saleQuery.data.isCancelled) {
    return <ContentContainer>{t('cancelledSaleCannotEdit')}</ContentContainer>
  }

  const hasCancelledItem = saleQuery.data.items.some((item) => item.isCancelled)

  return (
    <ContentContainer>
      <PageHeader title={t('editSale')} description={t('editSaleDescription')} />
      {hasCancelledItem ? (
        <div className="mb-4 rounded-2xl border border-amber-300/20 bg-amber-300/10 p-4 text-sm text-amber-100">
          {t('cancelledItemCannotEdit')}
        </div>
      ) : null}
      <SaleForm
        sale={saleQuery.data}
        submitLabel={t('saveChanges')}
        isSubmitting={mutation.isPending}
        isSubmitDisabled={hasCancelledItem}
        onSubmit={handleSubmit}
      />
    </ContentContainer>
  )
}
