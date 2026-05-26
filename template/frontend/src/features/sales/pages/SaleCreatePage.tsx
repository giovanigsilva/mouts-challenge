import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'

import { SaleForm } from '@/features/sales/components/SaleForm'
import { useCreateSale } from '@/features/sales/hooks/use-sales'
import type { SaleRequest } from '@/features/sales/types/sale.types'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'

export function SaleCreatePage() {
  const navigate = useNavigate()
  const mutation = useCreateSale()

  function handleSubmit(request: SaleRequest) {
    mutation.mutate(request, {
      onSuccess: (sale) => {
        toast.success('Venda criada com sucesso.')
        navigate(`/sales/${sale.id}`)
      },
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  return (
    <ContentContainer>
      <PageHeader
        title="Nova venda"
        description="Informe cliente, filial e itens. Descontos e totais são calculados novamente pelo backend."
      />
      <SaleForm submitLabel="Criar venda" isSubmitting={mutation.isPending} onSubmit={handleSubmit} />
    </ContentContainer>
  )
}
