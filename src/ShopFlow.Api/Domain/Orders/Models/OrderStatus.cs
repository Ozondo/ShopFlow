namespace ShopFlow.Api.Domain.Orders.Models;

public enum OrderStatus
{
    New = 0,
    Processing = 1,
    Shipped = 2,
    Cancelled = 3
}
