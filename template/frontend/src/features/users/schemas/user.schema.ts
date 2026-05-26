import { z } from 'zod'

export const createUserSchema = z.object({
  username: z.string().min(1, 'Informe o nome.'),
  email: z.string().email('Informe um email valido.'),
  phone: z.string().min(8, 'Informe o telefone.'),
  password: z.string().min(6, 'Informe uma senha com pelo menos 6 caracteres.'),
  status: z.number().min(1, 'Selecione o status.'),
  role: z.number().min(1, 'Selecione o perfil.'),
})

export type CreateUserFormValues = z.infer<typeof createUserSchema>
