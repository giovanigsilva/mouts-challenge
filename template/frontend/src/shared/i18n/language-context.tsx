import { useCallback, useMemo, useState, type PropsWithChildren } from 'react'

import { translations, type Language, type TranslationKey } from '@/shared/i18n/translations'
import { LanguageContext } from '@/shared/i18n/language-context-value'

const storageKey = 'developerstore.language'

export function LanguageProvider({ children }: PropsWithChildren) {
  const [language, setCurrentLanguage] = useState<Language>(() => readStoredLanguage())

  const setLanguage = useCallback((nextLanguage: Language) => {
    setCurrentLanguage(nextLanguage)
    localStorage.setItem(storageKey, nextLanguage)
  }, [])

  const t = useCallback(
    (key: TranslationKey, values?: Record<string, string | number>) => {
      const text = String(translations[language][key] ?? translations['pt-BR'][key] ?? key)

      if (!values) {
        return text
      }

      return Object.entries(values).reduce<string>((current, [name, value]) => current.replaceAll(`{${name}}`, String(value)), text)
    },
    [language],
  )

  const value = useMemo(() => ({ language, setLanguage, t }), [language, setLanguage, t])

  return <LanguageContext.Provider value={value}>{children}</LanguageContext.Provider>
}

function readStoredLanguage(): Language {
  const stored = localStorage.getItem(storageKey)

  if (stored === 'en' || stored === 'es' || stored === 'pt-BR') {
    return stored
  }

  return 'pt-BR'
}
