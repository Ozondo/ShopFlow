using System.Globalization;
using ShopFlow.Contracts.Order.V1;
using ShopFlow.OrderService.Domain.Orders.Models;
using Order = ShopFlow.OrderService.Domain.Orders.Models.Order;
using OrderItem = ShopFlow.Contracts.Order.V1.OrderItem;

namespace ShopFlow.OrderService.Endpoints;

public static class OrderMapper
{
    public static GetOrderResponse? Map(Order? order)
    {
        if (order == null) return null;
        
        return new GetOrderResponse
        {
            Id = order.Id.ToString(),
            CustomersName = order.CustomerName,
            Products = { order.Items.Select(x => new OrderItem
            {
                ProductId = x.ProductId.ToString(),
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice.ToString(CultureInfo.InvariantCulture),
            }) },
            Status = (OrderStatusEnum)order.Status,
            CreatedAt = order.CreatedAt.ToString(CultureInfo.InvariantCulture),
        };
    }
    
    public static List<GetOrderResponse>? Map(IEnumerable<Order>? orders)
    {
        if (orders == null) return null;
        
        return orders
            .Select(Map)
            .Cast<GetOrderResponse>()
            .ToList();
    }
}