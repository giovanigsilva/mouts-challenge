import { useNavigate, useParams } from 'react-router-dom'
import { toast } from 'sonner'

import { SaleForm } from '@/features/sales/components/SaleForm'
import { useSale, useUpdateSale } from '@/features/sales/hooks/use-sales'
import type { SaleRequest } from '@/features/sales/types/sale.types'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'

export function SaleEditPage() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const saleQuery = useSale(id)
  const mutation = useUpdateSale(id)

  function handleSubmit(request: SaleRequest) {
    mutation.mutate(request, {
      onSuccess: (sale) => {
        toast.success('Venda atualizada com sucesso.')
        navigate(`/sales/${sale.id}`)
      },
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  if (saleQuery.isLoading) {
    return <ContentContainer>Carregando venda...</ContentContainer>
  }

  if (saleQuery.error) {
    return <ContentContainer>{(saleQuery.error as unknown as NormalizedApiError).message}</ContentContainer>
  }

  if (!saleQuery.data) {
    return <ContentContainer>Venda nao encontrada.</ContentContainer>
  }

  if (saleQuery.data.isCancelled) {
    return <ContentContainer>Venda cancelada nao pode ser editada.</ContentContainer>
  }

  return (
    <ContentContainer>
      <PageHeader title="Editar venda" description="Atualize os dados da venda. O backend recalcula descontos e totais." />
      <SaleForm sale={saleQuery.data} submitLabel="Salvar alteracoes" isSubmitting={mutation.isPending} onSubmit={handleSubmit} />
    </ContentContainer>
  )
}
