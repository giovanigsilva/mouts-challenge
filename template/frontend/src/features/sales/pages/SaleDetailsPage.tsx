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

export function SaleDetailsPage() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const saleQuery = useSale(id)
  const cancelMutation = useCancelSale()
  const cancelItemMutation = useCancelSaleItem()
  const deleteMutation = useDeleteSale()
  const confirmDialog = useConfirmDialog()

  function handleCancelSale() {
    if (!saleQuery.data || !confirmDialog.confirm(`Cancelar a venda ${saleQuery.data.saleNumber}?`)) {
      return
    }

    cancelMutation.mutate(saleQuery.data.id, {
      onSuccess: () => toast.success('Venda cancelada com sucesso.'),
      onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
    })
  }

  function handleCancelItem(item: SaleItem) {
    if (!saleQuery.data || !confirmDialog.confirm(`Cancelar o item ${item.productName}?`)) {
      return
    }

    cancelItemMutation.mutate(
      { saleId: saleQuery.data.id, itemId: item.id },
      {
        onSuccess: () => toast.success('Item cancelado com sucesso.'),
        onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
      },
    )
  }

  function handleDelete() {
    if (!saleQuery.data || !confirmDialog.confirm(`Remover definitivamente a venda ${saleQuery.data.saleNumber}?`)) {
      return
    }

    deleteMutation.mutate(saleQuery.data.id, {
      onSuccess: () => {
        toast.success('Venda removida com sucesso.')
        navigate('/sales')
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

  const sale = saleQuery.data

  return (
    <ContentContainer>
      <PageHeader
        title={`Venda ${sale.saleNumber}`}
        description="Detalhes completos da venda, incluindo descontos, totais e itens cancelados."
        actions={
          <>
            <Button asChild variant="secondary" disabled={sale.isCancelled}>
              <Link to={`/sales/${sale.id}/edit`}>
                <Pencil className="h-4 w-4" />
                Editar
              </Link>
            </Button>
            <Button variant="secondary" disabled={sale.isCancelled} onClick={handleCancelSale}>
              <XCircle className="h-4 w-4" />
              Cancelar
            </Button>
            <Button variant="destructive" onClick={handleDelete}>
              <Trash2 className="h-4 w-4" />
              Remover
            </Button>
          </>
        }
      />
      <SaleDetailsPanel sale={sale} onCancelItem={handleCancelItem} />
    </ContentContainer>
  )
}
