using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.Api.Infrastructure.Interfaces;

public interface IOrdersRepository
{
    public Task<List<Order>> GetAll();
    public Task<Order?> GetById(Guid id);
    public Task<Order> Create(Order order);
    public Task<Order> Update(Order order);
}