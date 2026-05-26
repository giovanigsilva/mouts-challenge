export type CreateUserRequest = {
  username: string
  email: string
  phone: string
  password: string
  status: number
  role: number
}

export type UserResponse = {
  id: string
  username: string
  email: string
  phone: string
  status: number
  role: number
}
