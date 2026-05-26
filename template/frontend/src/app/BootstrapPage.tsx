import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { GlassCard } from '@/shared/components/glass/GlassCard'

export function BootstrapPage() {
  return (
    <ContentContainer>
      <PageHeader
        title="Frontend React inicializado"
        description="Base visual enterprise com layout, glassmorphism, Tailwind CSS, componentes reutilizáveis e navegação pronta para receber vendas, painel e saúde."
      />
      <GlassCard className="p-6">
        <p className="text-sm text-slate-300">
          Autenticação e rotas protegidas já estão ativas. As próximas etapas conectam as telas operacionais de vendas e saúde da API.
        </p>
      </GlassCard>
    </ContentContainer>
  )
}
