import { Activity, HeartPulse, LayoutDashboard, LogOut, Plus, ShoppingCart, UserPlus } from 'lucide-react'
import type { PropsWithChildren } from 'react'
import { NavLink } from 'react-router-dom'

import { Badge } from '@/shared/components/ui/badge'
import { Button } from '@/shared/components/ui/button'
import { PageTransition } from '@/shared/components/feedback/PageTransition'
import { cn } from '@/shared/lib/cn'

const navItems = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/sales', label: 'Vendas', icon: ShoppingCart },
  { to: '/sales/new', label: 'Nova venda', icon: Plus },
  { to: '/users/new', label: 'Criar usuario', icon: UserPlus },
  { to: '/health', label: 'Saude da API', icon: HeartPulse },
]

type AppShellProps = PropsWithChildren<{
  userLabel?: string
  onLogout?: () => void
}>

export function AppShell({ children, userLabel = 'Usuario autenticado', onLogout }: AppShellProps) {
  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <aside className="fixed inset-y-0 left-0 z-30 hidden w-72 border-r border-white/10 bg-slate-950/70 p-4 backdrop-blur-2xl lg:block">
        <div className="mb-8 rounded-2xl border border-cyan-300/20 bg-cyan-300/10 p-4">
          <p className="text-xs uppercase tracking-[0.24em] text-cyan-200">DeveloperStore</p>
          <p className="mt-2 text-lg font-semibold text-white">Sales Console</p>
        </div>
        <nav className="space-y-1">
          {navItems.map((item) => {
            const Icon = item.icon

            return (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  cn(
                    'flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium transition',
                    isActive ? 'bg-white/12 text-white' : 'text-slate-400 hover:bg-white/8 hover:text-white',
                  )
                }
              >
                <Icon className="h-4 w-4" />
                {item.label}
              </NavLink>
            )
          })}
        </nav>
      </aside>
      <div className="lg:pl-72">
        <header className="sticky top-0 z-20 border-b border-white/10 bg-slate-950/70 backdrop-blur-2xl">
          <div className="flex h-16 items-center justify-between px-4 sm:px-6 lg:px-8">
            <div className="flex items-center gap-3">
              <Activity className="h-5 w-5 text-cyan-300" />
              <div>
                <p className="text-sm font-semibold text-white">DeveloperStore Sales API</p>
                <p className="text-xs text-slate-500">http://localhost:8080</p>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <Badge variant="success">Development</Badge>
              <span className="hidden text-sm text-slate-300 sm:inline">{userLabel}</span>
              <Button variant="ghost" size="sm" onClick={onLogout}>
                <LogOut className="h-4 w-4" />
                Sair
              </Button>
            </div>
          </div>
        </header>
        <PageTransition>{children}</PageTransition>
      </div>
    </div>
  )
}
