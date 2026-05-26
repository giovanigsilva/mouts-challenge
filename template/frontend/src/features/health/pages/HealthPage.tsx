import { RefreshCw } from 'lucide-react'

import { HealthIndicator } from '@/features/health/components/HealthIndicator'
import { useHealthStatus } from '@/features/health/hooks/use-health'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { Button } from '@/shared/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card'
import { useLanguage } from '@/shared/i18n/use-language'
import type { TranslationKey } from '@/shared/i18n/translations'

export function HealthPage() {
  const queries = useHealthStatus()
  const { t } = useLanguage()

  function translateStatus(status: string) {
    if (status === 'Healthy') {
      return t('healthy')
    }

    if (status === 'Unavailable') {
      return t('unavailable')
    }

    return t('degraded')
  }

  function refresh() {
    queries.forEach((query) => void query.refetch())
  }

  return (
    <ContentContainer>
      <PageHeader
        title={t('apiHealth')}
        description={t('healthDescription')}
        actions={
          <Button type="button" variant="secondary" onClick={refresh}>
            <RefreshCw className="h-4 w-4" />
            {t('update')}
          </Button>
        }
      />
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {queries.map((query, index) => {
          const result = query.data

          return (
            <Card key={result?.key ?? index}>
              <CardHeader>
                <CardTitle>{result?.label ? t(result.label as TranslationKey) : t('apiHealth')}</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                {query.isLoading ? <p className="text-sm text-slate-400">{t('loading')}</p> : null}
                {result ? (
                  <>
                    <HealthIndicator status={result.status} />
                    <p className="text-sm text-slate-400">{t('monitoredService')}: {result.path}</p>
                    {result.data?.serviceName ? <p className="text-sm text-slate-400">{t('application')}: {result.data.serviceName}</p> : null}
                    <p className="text-sm text-slate-400">{t('time')}: {result.elapsedMs} ms</p>
                    <p className="break-all text-xs text-slate-500">{t('correlationId')}: {result.correlationId ?? '-'}</p>
                    {result.data?.healthChecks?.map((check) => (
                      <div key={check.name} className="rounded-xl bg-white/5 p-3 text-sm text-slate-300">
                        <p className="font-medium text-white">{check.name}</p>
                        <p>{translateStatus(check.status)}</p>
                        {check.description ? <p className="text-slate-500">{check.description}</p> : null}
                      </div>
                    ))}
                    {result.data?.services?.map((service) => (
                      <div key={service.name} className="rounded-xl bg-white/5 p-3 text-sm text-slate-300">
                        <div className="flex items-center justify-between gap-3">
                          <p className="font-medium text-white">{service.name}</p>
                          <HealthIndicator status={service.status === 'Healthy' ? 'Healthy' : 'Unhealthy'} />
                        </div>
                        {service.target ? <p className="mt-2 break-all text-xs text-slate-500">{service.target}</p> : null}
                      </div>
                    ))}
                  </>
                ) : null}
              </CardContent>
            </Card>
          )
        })}
      </div>
    </ContentContainer>
  )
}
