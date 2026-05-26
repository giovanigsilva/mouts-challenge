import { Plus, Trash2 } from 'lucide-react'
import { useFieldArray, type Control, type FieldErrors, type UseFormRegister } from 'react-hook-form'

import { calculateItemPreview } from '@/features/sales/lib/discount-preview'
import type { SaleFormValues } from '@/features/sales/schemas/sale.schema'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'
import { formatMoney } from '@/shared/lib/money'

type SaleItemsEditorProps = {
  control: Control<SaleFormValues>
  register: UseFormRegister<SaleFormValues>
  errors: FieldErrors<SaleFormValues>
  watchedItems: SaleFormValues['items']
}

export function SaleItemsEditor({ control, register, errors, watchedItems }: SaleItemsEditorProps) {
  const { fields, append, remove } = useFieldArray({ control, name: 'items' })
  const duplicatedProductMessage = 'Não é permitido repetir o mesmo produto na venda.'
  const duplicatedProductId = watchedItems
    .map((item) => item.productExternalId?.trim().toLowerCase())
    .find((productId, index, productIds) => productId && productIds.indexOf(productId) !== index)
  const duplicateWarning = duplicatedProductId ? duplicatedProductMessage : undefined
  const schemaItemsErrorMessage = errors.items?.message ?? errors.items?.root?.message
  const itemsErrorMessage = duplicateWarning ?? (schemaItemsErrorMessage === duplicatedProductMessage ? undefined : schemaItemsErrorMessage)

  return (
    <section className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold text-white">Itens</h2>
          <p className="text-sm text-slate-400">O desconto é calculado automaticamente pelo domínio. O front exibe apenas uma prévia.</p>
        </div>
        <Button
          type="button"
          variant="secondary"
          onClick={() => append({ productExternalId: '', productName: '', quantity: 1, unitPrice: 0 })}
        >
          <Plus className="h-4 w-4" />
          Item
        </Button>
      </div>

      {itemsErrorMessage ? <p className="text-sm text-rose-300">{itemsErrorMessage}</p> : null}

      <div className="space-y-3">
        {fields.map((field, index) => {
          const item = watchedItems[index] ?? field
          const preview = calculateItemPreview(Number(item.quantity || 0), Number(item.unitPrice || 0))

          return (
            <div key={field.id} className="rounded-2xl border border-white/10 bg-white/5 p-4">
              <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
                <div className="space-y-2 xl:col-span-2">
                  <Label htmlFor={`items.${index}.productExternalId`}>ID externo do produto</Label>
                  <Input id={`items.${index}.productExternalId`} {...register(`items.${index}.productExternalId`)} />
                  {errors.items?.[index]?.productExternalId ? (
                    <p className="text-sm text-rose-300">{errors.items[index]?.productExternalId?.message}</p>
                  ) : null}
                </div>
                <div className="space-y-2">
                  <Label htmlFor={`items.${index}.productName`}>Produto</Label>
                  <Input id={`items.${index}.productName`} {...register(`items.${index}.productName`)} />
                  {errors.items?.[index]?.productName ? <p className="text-sm text-rose-300">{errors.items[index]?.productName?.message}</p> : null}
                </div>
                <div className="space-y-2">
                  <Label htmlFor={`items.${index}.quantity`}>Quantidade</Label>
                  <Input id={`items.${index}.quantity`} type="number" min={1} max={20} {...register(`items.${index}.quantity`, { valueAsNumber: true })} />
                  {errors.items?.[index]?.quantity ? <p className="text-sm text-rose-300">{errors.items[index]?.quantity?.message}</p> : null}
                </div>
                <div className="space-y-2">
                  <Label htmlFor={`items.${index}.unitPrice`}>Preço unitário</Label>
                  <Input id={`items.${index}.unitPrice`} type="number" min={0} step="0.01" {...register(`items.${index}.unitPrice`, { valueAsNumber: true })} />
                  {errors.items?.[index]?.unitPrice ? <p className="text-sm text-rose-300">{errors.items[index]?.unitPrice?.message}</p> : null}
                </div>
              </div>
              <div className="mt-4 flex flex-wrap items-center justify-between gap-3 text-sm text-slate-300">
                <span>Desconto: {preview.discountPercentage}%</span>
                <span>Valor do desconto: {formatMoney(preview.discountAmount)}</span>
                <span>Total do item: {formatMoney(preview.totalAmount)}</span>
                <Button type="button" variant="ghost" size="sm" onClick={() => remove(index)} disabled={fields.length === 1}>
                  <Trash2 className="h-4 w-4" />
                  Remover
                </Button>
              </div>
            </div>
          )
        })}
      </div>
    </section>
  )
}
