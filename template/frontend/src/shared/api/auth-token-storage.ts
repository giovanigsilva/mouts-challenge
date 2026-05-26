const tokenKey = 'developerstore.accessToken'

export const authTokenStorage = {
  getToken() {
    return localStorage.getItem(tokenKey)
  },
  setToken(token: string) {
    localStorage.setItem(tokenKey, token)
  },
  clearToken() {
    localStorage.removeItem(tokenKey)
  },
  hasToken() {
    return Boolean(localStorage.getItem(tokenKey))
  },
}
