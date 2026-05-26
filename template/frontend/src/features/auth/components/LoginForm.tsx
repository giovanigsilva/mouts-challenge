import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { Loader2, ShieldCheck } from 'lucide-react'
import { useForm } from 'react-hook-form'
import { useNavigate } from 'react-router-dom'

import { login } from '@/features/auth/api/auth.api'
import { useAuth } from '@/features/auth/hooks/use-auth'
import { loginSchema, type LoginFormValues } from '@/features/auth/schemas/auth.schema'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { recaptchaConfig } from '@/shared/security/recaptcha/recaptcha.config'
import { useRecaptcha } from '@/shared/security/recaptcha/use-recaptcha'

export function LoginForm() {
  const { applyLogin } = useAuth()
  const navigate = useNavigate()
  const { executeRecaptcha } = useRecaptcha()
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
    },
  })

  const mutation = useMutation({
    mutationFn: login,
    onSuccess: (response) => {
      applyLogin(response)
      navigate('/dashboard', { replace: true })
    },
  })

  const apiError = mutation.error as NormalizedApiError | null

  function fillDemoCredentials() {
    setValue('email', 'admin@developerstore.com', { shouldValidate: true })
    setValue('password', 'Senha@123456', { shouldValidate: true })
  }

  async function submit(values: LoginFormValues) {
    const recaptchaToken = await executeRecaptcha(recaptchaConfig.loginAction)
    mutation.mutate({ ...values, recaptchaToken })
  }

  return (
    <form className="space-y-5" onSubmit={handleSubmit(submit)}>
      <div className="space-y-2">
        <Label htmlFor="email">E-mail</Label>
        <Input id="email" type="email" autoComplete="email" placeholder="admin@developerstore.com" {...register('email')} />
        {errors.email ? <p className="text-sm text-rose-300">{errors.email.message}</p> : null}
      </div>

      <div className="space-y-2">
        <Label htmlFor="password">Senha</Label>
        <Input id="password" type="password" autoComplete="current-password" placeholder="Senha@123456" {...register('password')} />
        {errors.password ? <p className="text-sm text-rose-300">{errors.password.message}</p> : null}
      </div>

      {apiError ? (
        <div className="rounded-xl border border-rose-300/20 bg-rose-300/10 p-3 text-sm text-rose-100">
          {apiError.message}
        </div>
      ) : null}

      <div className="rounded-2xl border border-cyan-300/20 bg-cyan-300/10 p-4 text-sm text-cyan-50 shadow-lg shadow-cyan-950/20">
        <div className="flex items-start gap-3">
          <div className="mt-0.5 rounded-xl border border-cyan-200/20 bg-cyan-200/15 p-2 text-cyan-100">
            <ShieldCheck className="h-5 w-5" aria-hidden="true" />
          </div>
          <div className="min-w-0 flex-1">
            <div className="flex flex-wrap items-center gap-2">
              <p className="font-semibold text-white">reCAPTCHA v3 simulado</p>
              <span className="rounded-full border border-white/10 bg-white/10 px-2 py-0.5 text-[11px] font-medium uppercase tracking-wide text-cyan-100">
                {recaptchaConfig.enabled ? 'Ativo' : 'Desativado'}
              </span>
            </div>
            <p className="mt-1 text-xs leading-5 text-slate-300">
              Proteção anti-bot local para login. Quando ativa, gera token por action sem usar Google real.
            </p>
            <p className="mt-2 text-[11px] uppercase tracking-wide text-cyan-200">
              Provedor: {recaptchaConfig.provider} | Ação: {recaptchaConfig.loginAction}
            </p>
          </div>
        </div>
      </div>

      <div className="flex flex-col gap-3 sm:flex-row">
        <Button type="submit" className="flex-1" disabled={mutation.isPending}>
          {mutation.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : null}
          {mutation.isPending ? 'Autenticando...' : 'Entrar'}
        </Button>
        <Button type="button" variant="secondary" onClick={fillDemoCredentials}>
          Usar demo
        </Button>
      </div>
    </form>
  )
}
