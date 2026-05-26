import { z } from 'zod'

export const loginSchema = z.object({
  email: z.string().min(1, 'Informe o email.').email('Informe um email valido.'),
  password: z.string().min(1, 'Informe a senha.'),
})

export type LoginFormValues = z.infer<typeof loginSchema>
