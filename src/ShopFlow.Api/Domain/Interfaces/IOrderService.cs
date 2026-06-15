using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Application.Common;
using ShopFlow.Api.Application.DTOs.Orders;
using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.Api.Application.Interfaces;

public interface IOrderService
{
    public Task<Result<IReadOnlyList<Order>>> GetAll();
    public Task<Result<Order>> GetById(Guid id);
    public Task<Result<Order>> Create(CreateOrderRequest request);
    public Task<Result<Order>> Update(Guid id, UpdateOrderStatusRequest request);
}