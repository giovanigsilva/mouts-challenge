import { QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import type { PropsWithChildren } from 'react'
import { Toaster } from 'sonner'

import { queryClient } from '@/app/query-client'
import { AuthProvider } from '@/features/auth/hooks/auth-provider'
import { LanguageProvider } from '@/shared/i18n/language-context'

export function AppProviders({ children }: PropsWithChildren) {
  return (
    <QueryClientProvider client={queryClient}>
      <LanguageProvider>
        <AuthProvider>{children}</AuthProvider>
      </LanguageProvider>
      <Toaster richColors position="top-right" />
      {import.meta.env.DEV ? <ReactQueryDevtools initialIsOpen={false} /> : null}
    </QueryClientProvider>
  )
}
