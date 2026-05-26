import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'

import { SaleForm } from '@/features/sales/components/SaleForm'
import { useCreateSale } from '@/features/sales/hooks/use-sales'
import type { SaleRequest } from '@/features/sales/types/sale.types'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { useLanguage } from '@/shared/i18n/use-language'

export function SaleCreatePage() {
  const navigate = useNavigate()
  const mutation = useCreateSale()
  const { t } = useLanguage()

  function handleSubmit(request: SaleRequest) {
    mutation.mutate(request, {
      onSuccess: (sale) => {
        toast.success(t('saleCreated'))
        navigate(`/sales/${sale.id}`)
      },
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  return (
    <ContentContainer>
      <PageHeader
        title={t('newSale')}
        description={t('saleCreateDescription')}
      />
      <SaleForm submitLabel={t('createSale')} isSubmitting={mutation.isPending} onSubmit={handleSubmit} />
    </ContentContainer>
  )
}
