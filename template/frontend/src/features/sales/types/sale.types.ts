export type SaleItemRequest = {
  productExternalId: string
  productName: string
  quantity: number
  unitPrice: number
}

export type SaleRequest = {
  saleNumber: string
  saleDate: string
  customerExternalId: string
  customerName: string
  branchExternalId: string
  branchName: string
  items: SaleItemRequest[]
}

export type SaleItem = SaleItemRequest & {
  id: string
  discountPercentage: number
  discountAmount: number
  totalAmount: number
  isCancelled: boolean
}

export type Sale = {
  id: string
  saleNumber: string
  saleDate: string
  customerExternalId: string
  customerName: string
  branchExternalId: string
  branchName: string
  totalAmount: number
  isCancelled: boolean
  createdAt: string
  updatedAt?: string | null
  items: SaleItem[]
}

export type SalesFilters = {
  page: number
  pageSize: number
  saleNumber?: string
  customerId?: string
  branchId?: string
  isCancelled?: boolean
  fromDate?: string
  toDate?: string
}

export type SalesPage = {
  items: Sale[]
  currentPage: number
  totalPages: number
  totalCount: number
}
