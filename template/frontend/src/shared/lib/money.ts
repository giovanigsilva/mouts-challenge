import { brCurrencyFormatter } from '@/shared/lib/formatters'

export function formatMoney(value: number) {
  return brCurrencyFormatter.format(value)
}
