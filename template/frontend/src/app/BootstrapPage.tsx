import { AppShell } from '@/shared/components/layout/AppShell'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { GlassCard } from '@/shared/components/glass/GlassCard'

export function BootstrapPage() {
  return (
    <AppShell>
      <ContentContainer>
        <PageHeader
          title="Frontend React inicializado"
          description="Base visual enterprise com layout, glassmorphism, Tailwind CSS, componentes reutilizáveis e navegação pronta para receber Auth, Sales, Dashboard e Health."
        />
        <GlassCard className="p-6">
          <p className="text-sm text-slate-300">
            A estrutura visual já está separada em componentes de layout, UI e glass. As próximas etapas conectam API, autenticação e fluxos de vendas.
          </p>
        </GlassCard>
      </ContentContainer>
    </AppShell>
  )
}
