import { Globe2 } from 'lucide-react'

import { useLanguage } from '@/shared/i18n/use-language'
import type { Language } from '@/shared/i18n/translations'
import { cn } from '@/shared/lib/cn'

const options: Array<{ language: Language; label: string }> = [
  { language: 'pt-BR', label: 'PT-BR' },
  { language: 'en', label: 'EN' },
  { language: 'es', label: 'ES' },
]

export function LanguageSelector() {
  const { language, setLanguage, t } = useLanguage()

  return (
    <div className="flex items-center gap-2 rounded-2xl border border-cyan-300/20 bg-slate-950/90 p-1.5 shadow-xl shadow-slate-950/30 backdrop-blur-xl" aria-label={t('language')}>
      <div className="hidden items-center gap-1.5 px-2 text-xs font-semibold text-cyan-100 sm:flex">
        <Globe2 className="h-3.5 w-3.5" />
        {t('language')}
      </div>
      {options.map((option) => (
        <button
          key={option.language}
          type="button"
          title={getTitle(option.language, t)}
          className={cn(
            'h-8 min-w-12 rounded-xl px-2 text-xs font-semibold transition',
            language === option.language ? 'bg-cyan-300 text-slate-950 shadow-lg shadow-cyan-950/30' : 'text-slate-300 hover:bg-white/10 hover:text-white',
          )}
          onClick={() => setLanguage(option.language)}
        >
          {option.label}
        </button>
      ))}
    </div>
  )
}

function getTitle(language: Language, t: ReturnType<typeof useLanguage>['t']) {
  if (language === 'en') {
    return t('languageEnglish')
  }

  if (language === 'es') {
    return t('languageSpanish')
  }

  return t('languagePortuguese')
}
