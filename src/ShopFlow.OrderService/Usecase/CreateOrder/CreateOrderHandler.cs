using System.Globalization;
using MediatR;
using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Usecase.CreateOrder;

public class CreateOrderHandler(IOrdersRepository ordersRepository, Product.ProductClient productClient) : IRequestHandler<CreateOrderCommand, Order>
{
    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var productsIds = request.Items.Select(x => x.ProductId).ToList();
        
        var itemsOrder = await  productClient.GetByIdsAsync(new GetByIdsRequest
        {
            Ids = { productsIds.Select(id => id.ToString()) }
        }, cancellationToken: cancellationToken);
        
        var productsDictionary = itemsOrder.Products
            .ToDictionary(x => x.Id);
        
        var orderItems = request.Items.Select(x =>
        {
            var product = productsDictionary[x.ProductId.ToString()];

            return new OrderItem(
                Guid.Parse(product.Id),
                product.Name,
                x.Quantity,
                decimal.Parse(product.Price, CultureInfo.InvariantCulture)
            );
        }).ToList();
        
        var order = new Order(
            Guid.NewGuid(),
            request.CustomerName,
            orderItems,
            OrderStatus.New,
            DateTime.UtcNow
        );
        
        return await ordersRepository.Create(order);
    }
}