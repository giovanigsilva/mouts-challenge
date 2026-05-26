import { brDateTimeFormatter } from '@/shared/lib/formatters'

export function formatDateTime(value?: string | null) {
  if (!value) {
    return '-'
  }

  return brDateTimeFormatter.format(new Date(value))
}
