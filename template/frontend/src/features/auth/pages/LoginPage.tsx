import { Navigate, Link } from 'react-router-dom'

import { LoginForm } from '@/features/auth/components/LoginForm'
import { useAuth } from '@/features/auth/hooks/use-auth'
import { PageTransition } from '@/shared/components/feedback/PageTransition'
import { GlassCard } from '@/shared/components/glass/GlassCard'
import { LanguageSelector } from '@/shared/components/layout/LanguageSelector'
import { useLanguage } from '@/shared/i18n/use-language'

export function LoginPage() {
  const { isAuthenticated } = useAuth()
  const { t } = useLanguage()

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />
  }

  return (
    <main className="flex min-h-screen items-center justify-center px-4 py-10 text-slate-100">
      <div className="fixed right-4 top-4 z-50">
        <LanguageSelector />
      </div>
      <PageTransition>
        <GlassCard className="w-full max-w-md p-7">
          <div className="mb-7">
            <p className="text-sm uppercase tracking-[0.24em] text-cyan-200">DeveloperStore</p>
            <h1 className="mt-3 text-3xl font-semibold text-white">{t('loginTitle')}</h1>
            <p className="mt-2 text-sm text-slate-400">{t('loginDescription')}</p>
          </div>
          <LoginForm />
          <p className="mt-6 text-center text-sm text-slate-400">
            {t('noUserYet')}{' '}
            <Link to="/users/new" className="font-medium text-cyan-200 hover:text-cyan-100">
              {t('createBySwaggerOrProtected')}
            </Link>
          </p>
        </GlassCard>
      </PageTransition>
    </main>
  )
}
