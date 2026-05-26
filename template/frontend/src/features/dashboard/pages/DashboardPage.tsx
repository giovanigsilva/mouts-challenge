import { Ban, CircleDollarSign, ShoppingCart, Store } from 'lucide-react'

import { HealthStatusCard } from '@/features/dashboard/components/HealthStatusCard'
import { MetricCard } from '@/features/dashboard/components/MetricCard'
import { RecentSalesCard } from '@/features/dashboard/components/RecentSalesCard'
import { useSalesList } from '@/features/sales/hooks/use-sales'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { formatMoney } from '@/shared/lib/money'

export function DashboardPage() {
  const salesQuery = useSalesList({ page: 1, pageSize: 5 })
  const sales = salesQuery.data?.items ?? []
  const activeSales = sales.filter((sale) => !sale.isCancelled)
  const cancelledSales = sales.filter((sale) => sale.isCancelled)
  const totalAmount = sales.reduce((sum, sale) => sum + sale.totalAmount, 0)

  return (
    <ContentContainer>
      <PageHeader
        title="Painel"
        description="Indicadores calculados sobre a página de vendas carregada. O backend não expõe endpoint agregado nesta versão."
      />
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <MetricCard title="Vendas realizadas" value={String(sales.length)} icon={ShoppingCart} />
        <MetricCard title="Vendas confirmadas" value={String(activeSales.length)} icon={Store} />
        <MetricCard title="Vendas canceladas" value={String(cancelledSales.length)} icon={Ban} />
        <MetricCard title="Total vendido" value={formatMoney(totalAmount)} icon={CircleDollarSign} />
      </div>
      <div className="mt-6 grid gap-4 xl:grid-cols-[1fr_2fr]">
        <HealthStatusCard />
        <RecentSalesCard sales={sales} />
      </div>
    </ContentContainer>
  )
}
