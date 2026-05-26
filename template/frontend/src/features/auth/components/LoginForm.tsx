import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { Loader2 } from 'lucide-react'
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
        <Label htmlFor="email">Email</Label>
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

      {recaptchaConfig.enabled ? <p className="text-xs text-cyan-200">Protecao anti-bot simulada ativa.</p> : null}

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
