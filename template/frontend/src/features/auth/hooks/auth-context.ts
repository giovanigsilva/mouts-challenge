import { createContext } from 'react'

import type { AuthUser, LoginResponse } from '@/features/auth/types/auth.types'

export type AuthContextValue = {
  user: AuthUser | null
  isAuthenticated: boolean
  applyLogin: (response: LoginResponse) => void
  logout: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)
