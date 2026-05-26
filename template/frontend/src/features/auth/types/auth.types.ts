export type LoginRequest = {
  email: string
  password: string
  recaptchaToken?: string | null
}

export type LoginResponse = {
  token: string
  email: string
  name: string
  role: string
}

export type AuthUser = {
  email: string
  name: string
  role: string
}
