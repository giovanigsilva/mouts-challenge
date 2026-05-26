import { Navigate, Link } from 'react-router-dom'

import { LoginForm } from '@/features/auth/components/LoginForm'
import { useAuth } from '@/features/auth/hooks/use-auth'
import { GlassCard } from '@/shared/components/glass/GlassCard'

export function LoginPage() {
  const { isAuthenticated } = useAuth()

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />
  }

  return (
    <main className="flex min-h-screen items-center justify-center px-4 py-10 text-slate-100">
      <GlassCard className="w-full max-w-md p-7">
        <div className="mb-7">
          <p className="text-sm uppercase tracking-[0.24em] text-cyan-200">DeveloperStore</p>
          <h1 className="mt-3 text-3xl font-semibold text-white">Entrar no console</h1>
          <p className="mt-2 text-sm text-slate-400">
            Use um usuario criado na API para acessar vendas, dashboard e health checks.
          </p>
        </div>
        <LoginForm />
        <p className="mt-6 text-center text-sm text-slate-400">
          Ainda nao tem usuario?{' '}
          <Link to="/users/new" className="font-medium text-cyan-200 hover:text-cyan-100">
            Crie pelo Swagger ou pela tela protegida.
          </Link>
        </p>
      </GlassCard>
    </main>
  )
}
