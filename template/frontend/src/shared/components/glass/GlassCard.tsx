import type { HTMLAttributes } from 'react'

import { cn } from '@/shared/lib/cn'

export function GlassCard({ className, ...props }: HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      className={cn('rounded-2xl border border-white/10 bg-white/[0.06] shadow-2xl shadow-black/30 backdrop-blur-xl', className)}
      {...props}
    />
  )
}
