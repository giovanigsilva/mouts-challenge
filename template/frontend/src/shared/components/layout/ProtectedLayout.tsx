import { Navigate, Outlet } from 'react-router-dom'

import { AppShell } from '@/shared/components/layout/AppShell'
import { useAuth } from '@/features/auth/hooks/use-auth'
import { useLanguage } from '@/shared/i18n/use-language'

export function ProtectedLayout() {
  const { isAuthenticated, user, logout } = useAuth()
  const { t } = useLanguage()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return (
    <AppShell userLabel={user?.name || user?.email || t('authenticatedUser')} userRole={user?.role} onLogout={logout}>
      <Outlet />
    </AppShell>
  )
}
