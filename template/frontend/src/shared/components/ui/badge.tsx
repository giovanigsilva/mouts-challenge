import type { HTMLAttributes } from 'react'

import { cn } from '@/shared/lib/cn'

type BadgeVariant = 'default' | 'success' | 'warning' | 'danger' | 'muted'

const variants: Record<BadgeVariant, string> = {
  default: 'border-cyan-300/30 bg-cyan-300/10 text-cyan-200',
  success: 'border-emerald-300/30 bg-emerald-300/10 text-emerald-200',
  warning: 'border-amber-300/30 bg-amber-300/10 text-amber-200',
  danger: 'border-rose-300/30 bg-rose-300/10 text-rose-200',
  muted: 'border-white/10 bg-white/5 text-slate-300',
}

type BadgeProps = HTMLAttributes<HTMLSpanElement> & {
  variant?: BadgeVariant
}

export function Badge({ className, variant = 'default', ...props }: BadgeProps) {
  return (
    <span
      className={cn('inline-flex items-center rounded-full border px-2.5 py-1 text-xs font-medium', variants[variant], className)}
      {...props}
    />
  )
}
