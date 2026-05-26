import { z } from 'zod'

import { translations, type TranslationKey } from '@/shared/i18n/translations'

type Translate = (key: TranslationKey) => string

export function createSaleSchema(t: Translate) {
  const saleItemSchema = z.object({
    productExternalId: z.string().uuid(t('validationUuid')),
    productName: z.string().min(1, t('validationProductName')).max(120, t('validationMax120')),
    quantity: z.number().int(t('validationIntegerQuantity')).min(1, t('validationMinQuantity')).max(20, t('validationMaxQuantity')),
    unitPrice: z.number().positive(t('validationUnitPrice')),
  })

  return z
    .object({
      saleNumber: z.string().min(1, t('validationSaleNumber')).max(50, t('validationMax50')),
      saleDate: z.string().min(1, t('validationSaleDate')),
      customerExternalId: z.string().uuid(t('validationUuid')),
      customerName: z.string().min(1, t('validationCustomer')).max(120, t('validationMax120')),
      branchExternalId: z.string().uuid(t('validationUuid')),
      branchName: z.string().min(1, t('validationBranch')).max(120, t('validationMax120')),
      items: z.array(saleItemSchema).min(1, t('validationItems')),
    })
    .superRefine((value, context) => {
      const duplicated = value.items.find((item, index) => value.items.findIndex((inner) => inner.productExternalId === item.productExternalId) !== index)

      if (duplicated) {
        context.addIssue({
          code: 'custom',
          path: ['items'],
          message: t('duplicatedProduct'),
        })
      }
    })
}

export const saleSchema = createSaleSchema((key) => translations['pt-BR'][key])

export type SaleFormValues = z.infer<ReturnType<typeof createSaleSchema>>
