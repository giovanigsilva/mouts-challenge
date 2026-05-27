using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Reports;

public sealed class SalesByUserReport
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int TotalSales { get; set; }
    public int ActiveSales { get; set; }
    public int CancelledSales { get; set; }
    public decimal TotalSoldAmount { get; set; }
    public DateTime? FirstSaleDate { get; set; }
    public DateTime? LastSaleDate { get; set; }
}
