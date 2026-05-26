import { Link } from 'react-router-dom'

import type { Sale } from '@/features/sales/types/sale.types'
import { GlassCard } from '@/shared/components/glass/GlassCard'
import { formatMoney } from '@/shared/lib/money'

type RecentSalesCardProps = {
  sales: Sale[]
}

export function RecentSalesCard({ sales }: RecentSalesCardProps) {
  return (
    <GlassCard className="p-5">
      <h2 className="text-lg font-semibold text-white">Vendas recentes</h2>
      <p className="mt-1 text-sm text-slate-400">Dados calculados sobre a pagina carregada.</p>
      <div className="mt-4 space-y-3">
        {sales.map((sale) => (
          <Link key={sale.id} to={`/sales/${sale.id}`} className="flex items-center justify-between rounded-xl bg-white/5 p-3 hover:bg-white/10">
            <span className="text-sm font-medium text-white">{sale.saleNumber}</span>
            <span className="text-sm text-slate-300">{formatMoney(sale.totalAmount)}</span>
          </Link>
        ))}
        {!sales.length ? <p className="text-sm text-slate-400">Nenhuma venda carregada.</p> : null}
      </div>
    </GlassCard>
  )
}
