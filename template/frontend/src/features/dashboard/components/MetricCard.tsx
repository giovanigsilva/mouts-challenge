import type { LucideIcon } from 'lucide-react'

import { GlassCard } from '@/shared/components/glass/GlassCard'

type MetricCardProps = {
  title: string
  value: string
  description?: string
  icon: LucideIcon
}

export function MetricCard({ title, value, description, icon: Icon }: MetricCardProps) {
  return (
    <GlassCard className="p-5">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-sm text-slate-400">{title}</p>
          <p className="mt-2 text-3xl font-semibold text-white">{value}</p>
          {description ? <p className="mt-1 text-xs text-slate-500">{description}</p> : null}
        </div>
        <div className="rounded-xl bg-cyan-300/10 p-3 text-cyan-200">
          <Icon className="h-5 w-5" />
        </div>
      </div>
    </GlassCard>
  )
}
