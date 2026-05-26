import { useCallback, useMemo, useState, type PropsWithChildren } from 'react'
import { toast } from 'sonner'

import { queryClient } from '@/app/query-client'
import { authTokenStorage } from '@/shared/api/auth-token-storage'
import { AuthContext } from '@/features/auth/hooks/auth-context'
import type { AuthUser, LoginResponse } from '@/features/auth/types/auth.types'

const userKey = 'developerstore.user'

function readStoredUser() {
  const raw = localStorage.getItem(userKey)

  if (!raw) {
    return null
  }

  try {
    return JSON.parse(raw) as AuthUser
  } catch {
    localStorage.removeItem(userKey)
    return null
  }
}

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<AuthUser | null>(() => readStoredUser())
  const isAuthenticated = authTokenStorage.hasToken()

  const applyLogin = useCallback((response: LoginResponse) => {
    const nextUser = {
      email: response.email,
      name: response.name,
      role: response.role,
    }

    authTokenStorage.setToken(response.token)
    localStorage.setItem(userKey, JSON.stringify(nextUser))
    setUser(nextUser)
    toast.success('Autenticação realizada com sucesso.')
  }, [])

  const logout = useCallback(() => {
    authTokenStorage.clearToken()
    localStorage.removeItem(userKey)
    setUser(null)
    queryClient.clear()
    window.location.assign('/login')
  }, [])

  const value = useMemo(
    () => ({
      user,
      isAuthenticated,
      applyLogin,
      logout,
    }),
    [applyLogin, isAuthenticated, logout, user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
