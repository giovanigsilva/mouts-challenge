export function getDiscountPercentage(quantity: number) {
  if (quantity >= 10) {
    return 20
  }

  if (quantity >= 4) {
    return 10
  }

  return 0
}

export function calculateItemPreview(quantity: number, unitPrice: number) {
  const grossAmount = quantity * unitPrice
  const discountPercentage = getDiscountPercentage(quantity)
  const discountAmount = (grossAmount * discountPercentage) / 100

  return {
    discountPercentage,
    discountAmount,
    totalAmount: grossAmount - discountAmount,
  }
}
