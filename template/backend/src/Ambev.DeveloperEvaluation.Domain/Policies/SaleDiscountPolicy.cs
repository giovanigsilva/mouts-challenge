using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Policies;

public static class SaleDiscountPolicy
{
    public static decimal GetDiscountPercentage(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleException("A quantidade do item deve ser maior que zero.");

        if (quantity <= 3)
            return 0m;

        if (quantity <= 9)
            return 10m;

        if (quantity <= 20)
            return 20m;

        throw new BusinessRuleException("Nao e permitido vender mais de 20 unidades identicas.");
    }
}
