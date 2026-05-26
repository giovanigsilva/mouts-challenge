export function useConfirmDialog() {
  return {
    confirm(message: string) {
      return window.confirm(message)
    },
  }
}
