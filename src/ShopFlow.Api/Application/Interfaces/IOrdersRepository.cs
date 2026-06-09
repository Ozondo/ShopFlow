using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.Api.Domain.Products.Models;

namespace ShopFlow.Api.Application.Interfaces;

public interface IOrdersRepository
{
    public Task<List<Order>> GetAll();
    public Task<Order?> GetById(Guid id);
    public Task<Order> Create(Order order);
    public Task<Order?> Update(Guid id, OrderStatus orderStatus);
}