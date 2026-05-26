import { Navigate, Outlet } from 'react-router-dom'

import { AppShell } from '@/shared/components/layout/AppShell'
import { useAuth } from '@/features/auth/hooks/use-auth'

export function ProtectedLayout() {
  const { isAuthenticated, user, logout } = useAuth()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return (
    <AppShell userLabel={user?.name || user?.email || 'Usuário autenticado'} onLogout={logout}>
      <Outlet />
    </AppShell>
  )
}
