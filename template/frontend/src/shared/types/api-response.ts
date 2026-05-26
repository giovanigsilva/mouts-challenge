export type ApiEnvelope<T = unknown> = {
  success: boolean
  message?: string
  data?: T
  errors?: ApiFieldError[] | string[] | Record<string, string[]>
  correlationId?: string
  timestamp?: string
}

export type ApiFieldError = {
  field?: string
  message: string
}
