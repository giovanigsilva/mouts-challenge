import { z } from 'zod'

const uuidMessage = 'Informe um identificador UUID válido.'

export const saleItemSchema = z.object({
  productExternalId: z.string().uuid(uuidMessage),
  productName: z.string().min(1, 'Informe o nome do produto.').max(120, 'Use no máximo 120 caracteres.'),
  quantity: z.number().int('Informe uma quantidade inteira.').min(1, 'Quantidade mínima: 1.').max(20, 'Quantidade máxima: 20.'),
  unitPrice: z.number().positive('Informe um preço unitário maior que zero.'),
})

export const saleSchema = z
  .object({
    saleNumber: z.string().min(1, 'Informe o número da venda.').max(50, 'Use no máximo 50 caracteres.'),
    saleDate: z.string().min(1, 'Informe a data da venda.'),
    customerExternalId: z.string().uuid(uuidMessage),
    customerName: z.string().min(1, 'Informe o cliente.').max(120, 'Use no máximo 120 caracteres.'),
    branchExternalId: z.string().uuid(uuidMessage),
    branchName: z.string().min(1, 'Informe a filial.').max(120, 'Use no máximo 120 caracteres.'),
    items: z.array(saleItemSchema).min(1, 'Informe pelo menos um item.'),
  })
  .superRefine((value, context) => {
    const duplicated = value.items.find((item, index) => value.items.findIndex((inner) => inner.productExternalId === item.productExternalId) !== index)

    if (duplicated) {
      context.addIssue({
        code: 'custom',
        path: ['items'],
        message: 'Não é permitido repetir o mesmo produto na venda.',
      })
    }
  })

export type SaleFormValues = z.infer<typeof saleSchema>
