import type { HTMLAttributes } from 'react'

import { cn } from '@/shared/lib/cn'

export function GlassPanel({ className, ...props }: HTMLAttributes<HTMLDivElement>) {
  return <div className={cn('border border-white/10 bg-slate-950/40 backdrop-blur-2xl', className)} {...props} />
}
