export type LoginRequest = {
  email: string
  password: string
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
