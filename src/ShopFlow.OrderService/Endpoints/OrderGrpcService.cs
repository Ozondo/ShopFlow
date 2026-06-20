using System.Globalization;
using Grpc.Core;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Infrastructure.Interfaces;
using ShopFlow.Contracts.Order.V1;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.OrderService.Domain.Orders.Models;
using OrderStatus = ShopFlow.Api.Domain.Orders.Models.OrderStatus;

namespace ShopFlow.OrderService.Endpoints;

public class OrderGrpcService(IOrdersRepository ordersRepository, Product.ProductClient productClient): Order.OrderBase
{
    public override async Task<GetAllOrderResponse> GetAll(GetAllOrderRequest request, ServerCallContext context)
    {
        var result = await ordersRepository.GetAll();
        
        var response = new GetAllOrderResponse();
        
        response.Orders.AddRange(OrderMapper.Map(result));
        
        return response;
    }

    public override async Task<GetOrderResponse?> GetOrder(GetOrderRequest request, ServerCallContext context)
    {
        var result = await ordersRepository.GetById(Guid.Parse(request.Id));
        
        return OrderMapper.Map(result);
    }

    public override async Task<GetOrderResponse?> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var productsIds = request.Items.Select(x => x.ProductId).ToList();
        
        Console.WriteLine($"Products from ProductService: {productsIds.Count}");

        var itemsOrder = await productClient.GetByIdsAsync(new GetByIdsRequest
        {
            Ids = { productsIds }
        });
        
        Console.WriteLine(itemsOrder.Products.Count);
        

        var productsDictionary = itemsOrder.Products
            .ToDictionary(x => x.Id);
        
        var orderItems = request.Items.Select(x =>
        {
            var product = productsDictionary[x.ProductId];

            return new OrderItemDTO(
                Guid.Parse(product.Id),
                product.Name,
                x.Quantity,
                decimal.Parse(product.Price, CultureInfo.InvariantCulture)
            );
        }).ToList();
        
        Console.WriteLine($"Products from ProductService: {itemsOrder.Products.Count}");
        Console.WriteLine($"Order items created: {orderItems.Count}");

        var order = new OrderDTO(
            Guid.NewGuid(),
            request.CustomerName,
            orderItems,
            OrderStatus.New,
            DateTime.UtcNow
        );
        
        var result = await ordersRepository.Create(order);
        
        Console.WriteLine($"Saved order items: {result.Items.Count}");
        
        return OrderMapper.Map(result);
    }

    public override async Task<GetOrderResponse?> UpdateOrderStatus(UpdateStatusRequest request, ServerCallContext context)
    {
        var order = await ordersRepository.GetById(Guid.Parse(request.Id));

        if (order == null) return null;

        var newOrder = new OrderDTO(order.Id, order.CustomerName, order.Items, (OrderStatus)request.OrderStatus, order.CreatedAt);
        
        var result = await ordersRepository.Update(newOrder);
        
        return OrderMapper.Map(result);
    }
}