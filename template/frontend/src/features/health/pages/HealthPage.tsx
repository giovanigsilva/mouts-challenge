import { RefreshCw } from 'lucide-react'

import { HealthIndicator } from '@/features/health/components/HealthIndicator'
import { useHealthStatus } from '@/features/health/hooks/use-health'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { Button } from '@/shared/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card'

export function HealthPage() {
  const queries = useHealthStatus()

  function refresh() {
    queries.forEach((query) => void query.refetch())
  }

  return (
    <ContentContainer>
      <PageHeader
        title="Saude da API"
        description="Consulta endpoints de health do backend e mede o tempo de resposta observado pelo frontend."
        actions={
          <Button type="button" variant="secondary" onClick={refresh}>
            <RefreshCw className="h-4 w-4" />
            Atualizar
          </Button>
        }
      />
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {queries.map((query, index) => {
          const result = query.data

          return (
            <Card key={result?.key ?? index}>
              <CardHeader>
                <CardTitle>{result?.label ?? 'Health'}</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                {query.isLoading ? <p className="text-sm text-slate-400">Carregando...</p> : null}
                {result ? (
                  <>
                    <HealthIndicator status={result.status} />
                    <p className="text-sm text-slate-400">Tempo: {result.elapsedMs} ms</p>
                    <p className="break-all text-xs text-slate-500">CorrelationId: {result.correlationId ?? '-'}</p>
                    {result.data?.healthChecks?.map((check) => (
                      <div key={check.name} className="rounded-xl bg-white/5 p-3 text-sm text-slate-300">
                        <p className="font-medium text-white">{check.name}</p>
                        <p>{check.status}</p>
                        {check.description ? <p className="text-slate-500">{check.description}</p> : null}
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
