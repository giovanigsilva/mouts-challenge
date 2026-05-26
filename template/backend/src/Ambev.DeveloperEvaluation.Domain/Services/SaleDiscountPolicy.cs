namespace Ambev.DeveloperEvaluation.Domain.Services;

public static class SaleDiscountPolicy
{
    public static decimal GetDiscountPercentage(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessRuleException("A quantidade do item deve ser maior que zero.");

        if (quantity > 20)
            throw new BusinessRuleException("Nao e permitido vender mais de 20 itens identicos.");

        if (quantity >= 10)
            return 20m;

        if (quantity >= 4)
            return 10m;

        return 0m;
    }
}
