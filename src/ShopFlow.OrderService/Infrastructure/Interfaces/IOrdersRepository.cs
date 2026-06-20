using ShopFlow.Api.Domain.Orders.Models;
using ShopFlow.OrderService.Domain.Orders.Models;

namespace ShopFlow.Api.Infrastructure.Interfaces;

public interface IOrdersRepository
{
    public Task<List<OrderDTO>> GetAll();
    public Task<OrderDTO?> GetById(Guid id);
    public Task<OrderDTO> Create(OrderDTO orderDto);
    public Task<OrderDTO> Update(OrderDTO orderDto);
}