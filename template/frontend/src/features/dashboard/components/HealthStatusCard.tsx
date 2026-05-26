import { HeartPulse } from 'lucide-react'

import { HealthIndicator } from '@/features/health/components/HealthIndicator'
import { useHealthStatus } from '@/features/health/hooks/use-health'
import { GlassCard } from '@/shared/components/glass/GlassCard'
import { useLanguage } from '@/shared/i18n/use-language'

export function HealthStatusCard() {
  const queries = useHealthStatus()
  const ready = queries.find((query) => query.data?.key === 'ready')?.data
  const { t } = useLanguage()

  return (
    <GlassCard className="p-5">
      <div className="flex items-center gap-3">
        <div className="rounded-xl bg-emerald-300/10 p-3 text-emerald-200">
          <HeartPulse className="h-5 w-5" />
        </div>
        <div>
          <p className="text-sm text-slate-400">{t('readiness')}</p>
          <div className="mt-2">{ready ? <HealthIndicator status={ready.status} /> : <span className="text-sm text-slate-500">{t('loading')}</span>}</div>
        </div>
      </div>
    </GlassCard>
  )
}
