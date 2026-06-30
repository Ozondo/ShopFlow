using ShopFlow.OrderService.Domain.Orders.Models;

namespace ShopFlow.OrderService.Infrastructure.Interfaces;

public interface IOrdersRepository
{
    public Task<List<Order>> GetAll();
    public Task<Order?> GetById(Guid id);
    public Task<Order> Create(Order order);
    public Task<Order> Update(Order order);
}