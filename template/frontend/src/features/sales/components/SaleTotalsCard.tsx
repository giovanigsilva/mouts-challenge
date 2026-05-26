import { ShoppingCart } from 'lucide-react'

import { GlassCard } from '@/shared/components/glass/GlassCard'
import { useLanguage } from '@/shared/i18n/use-language'
import { formatMoney } from '@/shared/lib/money'

type SaleTotalsCardProps = {
  totalAmount: number
  itemsCount: number
}

export function SaleTotalsCard({ totalAmount, itemsCount }: SaleTotalsCardProps) {
  const { t } = useLanguage()

  return (
    <GlassCard className="p-5">
      <div className="flex items-center gap-3">
        <div className="rounded-xl bg-cyan-300/10 p-3 text-cyan-200">
          <ShoppingCart className="h-5 w-5" />
        </div>
        <div>
          <p className="text-sm text-slate-400">{t('saleTotal')}</p>
          <p className="text-2xl font-semibold text-white">{formatMoney(totalAmount)}</p>
          <p className="text-xs text-slate-500">{itemsCount} item(ns)</p>
        </div>
      </div>
    </GlassCard>
  )
}
