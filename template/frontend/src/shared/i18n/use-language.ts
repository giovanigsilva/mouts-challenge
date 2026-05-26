import { useContext } from 'react'

import { LanguageContext } from '@/shared/i18n/language-context-value'

export function useLanguage() {
  const context = useContext(LanguageContext)

  if (!context) {
    throw new Error('useLanguage deve ser usado dentro de LanguageProvider.')
  }

  return context
}
