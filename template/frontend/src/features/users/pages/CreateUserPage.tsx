import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { Loader2 } from 'lucide-react'
import type { ReactNode } from 'react'
import { useForm } from 'react-hook-form'
import { toast } from 'sonner'

import { createUser } from '@/features/users/api/users.api'
import { createUserSchema, type CreateUserFormValues } from '@/features/users/schemas/user.schema'
import type { NormalizedApiError } from '@/shared/api/api-error'
import { ContentContainer } from '@/shared/components/layout/ContentContainer'
import { PageHeader } from '@/shared/components/layout/PageHeader'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'

export function CreateUserPage() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateUserFormValues>({
    resolver: zodResolver(createUserSchema),
    defaultValues: {
      username: 'Admin DeveloperStore',
      email: 'admin@developerstore.com',
      phone: '11999999999',
      password: 'Senha@123456',
      status: 1,
      role: 3,
    },
  })

  const mutation = useMutation({
    mutationFn: createUser,
    onSuccess: () => toast.success('Usuario criado com sucesso.'),
    onError: (error) => toast.error((error as unknown as NormalizedApiError).message),
  })

  return (
    <ContentContainer>
      <PageHeader title="Criar usuario" description="Cria usuario para autenticar na API DeveloperStore." />
      <form className="grid gap-4 rounded-2xl border border-white/10 bg-white/5 p-5 md:grid-cols-2" onSubmit={handleSubmit((values) => mutation.mutate(values))}>
        <Field label="Nome" id="username" error={errors.username?.message}>
          <Input id="username" {...register('username')} />
        </Field>
        <Field label="Email" id="email" error={errors.email?.message}>
          <Input id="email" type="email" {...register('email')} />
        </Field>
        <Field label="Telefone" id="phone" error={errors.phone?.message}>
          <Input id="phone" {...register('phone')} />
        </Field>
        <Field label="Senha" id="password" error={errors.password?.message}>
          <Input id="password" type="password" {...register('password')} />
        </Field>
        <Field label="Status" id="status" error={errors.status?.message}>
          <select id="status" className="h-11 w-full rounded-xl border border-white/10 bg-white/10 px-3 text-sm text-slate-100 outline-none" {...register('status', { valueAsNumber: true })}>
            <option value={1}>Active</option>
            <option value={2}>Inactive</option>
            <option value={3}>Suspended</option>
          </select>
        </Field>
        <Field label="Perfil" id="role" error={errors.role?.message}>
          <select id="role" className="h-11 w-full rounded-xl border border-white/10 bg-white/10 px-3 text-sm text-slate-100 outline-none" {...register('role', { valueAsNumber: true })}>
            <option value={1}>Customer</option>
            <option value={2}>Manager</option>
            <option value={3}>Admin</option>
          </select>
        </Field>
        <div className="md:col-span-2">
          <Button type="submit" disabled={mutation.isPending}>
            {mutation.isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : null}
            Criar usuario
          </Button>
        </div>
      </form>
    </ContentContainer>
  )
}

function Field({ label, id, error, children }: { label: string; id: string; error?: string; children: ReactNode }) {
  return (
    <div className="space-y-2">
      <Label htmlFor={id}>{label}</Label>
      {children}
      {error ? <p className="text-sm text-rose-300">{error}</p> : null}
    </div>
  )
}
