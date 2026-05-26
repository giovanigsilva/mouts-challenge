import { zodResolver } from '@hookform/resolvers/zod'
import { Loader2 } from 'lucide-react'
import type { ReactNode } from 'react'
import { useForm, useWatch } from 'react-hook-form'

import { SaleItemsEditor } from '@/features/sales/components/SaleItemsEditor'
import { saleSchema, type SaleFormValues } from '@/features/sales/schemas/sale.schema'
import type { Sale, SaleRequest } from '@/features/sales/types/sale.types'
import { Button } from '@/shared/components/ui/button'
import { Input } from '@/shared/components/ui/input'
import { Label } from '@/shared/components/ui/label'

type SaleFormProps = {
  sale?: Sale
  isSubmitting?: boolean
  submitLabel: string
  onSubmit: (request: SaleRequest) => void
}

export function SaleForm({ sale, isSubmitting = false, submitLabel, onSubmit }: SaleFormProps) {
  const {
    register,
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<SaleFormValues>({
    resolver: zodResolver(saleSchema),
    defaultValues: sale ? saleToFormValues(sale) : createDefaultValues(),
  })

  const watchedItems = useWatch({ control, name: 'items' })

  function submit(values: SaleFormValues) {
    onSubmit({
      ...values,
      saleDate: new Date(values.saleDate).toISOString(),
      items: values.items.map((item) => ({
        productExternalId: item.productExternalId,
        productName: item.productName,
        quantity: Number(item.quantity),
        unitPrice: Number(item.unitPrice),
      })),
    })
  }

  return (
    <form className="space-y-6" onSubmit={handleSubmit(submit)}>
      <div className="grid gap-4 rounded-2xl border border-white/10 bg-white/5 p-4 md:grid-cols-2">
        <Field label="Numero da venda" id="saleNumber" error={errors.saleNumber?.message}>
          <Input id="saleNumber" {...register('saleNumber')} />
        </Field>
        <Field label="Data da venda" id="saleDate" error={errors.saleDate?.message}>
          <Input id="saleDate" type="datetime-local" {...register('saleDate')} />
        </Field>
        <Field label="CustomerExternalId" id="customerExternalId" error={errors.customerExternalId?.message}>
          <Input id="customerExternalId" {...register('customerExternalId')} />
        </Field>
        <Field label="Cliente" id="customerName" error={errors.customerName?.message}>
          <Input id="customerName" {...register('customerName')} />
        </Field>
        <Field label="BranchExternalId" id="branchExternalId" error={errors.branchExternalId?.message}>
          <Input id="branchExternalId" {...register('branchExternalId')} />
        </Field>
        <Field label="Filial" id="branchName" error={errors.branchName?.message}>
          <Input id="branchName" {...register('branchName')} />
        </Field>
      </div>

      <SaleItemsEditor control={control} register={register} errors={errors} watchedItems={watchedItems} />

      <div className="flex justify-end">
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : null}
          {submitLabel}
        </Button>
      </div>
    </form>
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

function createDefaultValues(): SaleFormValues {
  return {
    saleNumber: `SALE-${new Date().getFullYear()}-${String(Date.now()).slice(-6)}`,
    saleDate: toDateTimeLocalValue(new Date().toISOString()),
    customerExternalId: '',
    customerName: '',
    branchExternalId: '',
    branchName: '',
    items: [{ productExternalId: '', productName: '', quantity: 1, unitPrice: 0 }],
  }
}

function saleToFormValues(sale: Sale): SaleFormValues {
  return {
    saleNumber: sale.saleNumber,
    saleDate: toDateTimeLocalValue(sale.saleDate),
    customerExternalId: sale.customerExternalId,
    customerName: sale.customerName,
    branchExternalId: sale.branchExternalId,
    branchName: sale.branchName,
    items: sale.items.map((item) => ({
      productExternalId: item.productExternalId,
      productName: item.productName,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
    })),
  }
}

function toDateTimeLocalValue(value: string) {
  const date = new Date(value)
  const offset = date.getTimezoneOffset()
  const localDate = new Date(date.getTime() - offset * 60_000)

  return localDate.toISOString().slice(0, 16)
}
