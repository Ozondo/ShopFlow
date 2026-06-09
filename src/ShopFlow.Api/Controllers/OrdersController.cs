using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Application.DTOs.Orders;
using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.Api.Controllers;

/// <summary>
/// TODO: реализуй работу с заказами.
/// Эндпоинты: GET /api/orders, GET /api/orders/{id}, POST, PATCH /api/orders/{id}/status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    // TODO: внедри IOrderRepository через конструктор
    private readonly IOrdersRepository _ordersRepository;
    private readonly IProductRepository _productRepository;
    public OrdersController(IOrdersRepository ordersRepository, IProductRepository productRepository)
    {
        _ordersRepository = ordersRepository;
        _productRepository = productRepository;
    }
    
    // TODO: добавь action-методы и бизнес-правила (валидация stock, переходы статусов)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _ordersRepository.GetAll();
        
        return Ok(orders);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _ordersRepository.GetById(id);
        
        if(result == null) return NotFound($"Order with id {id} not found");
        
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest orderRequest)
    {
        var items  = await _productRepository.GetAll();
        
        var orderItems = new List<OrderItem>();

        foreach (var item in orderRequest.Items)
        {
            var product = items.FirstOrDefault(x => x.Id == item.ProductId);
            
            if (product == null) return NotFound($"Product with id {item.ProductId} not found");
            if (product.Stock == 0) return BadRequest($"Product with id {item.ProductId} stock is 0");
            if (product.Stock < item.Quantity) return BadRequest($"Product with id {item.ProductId} not enough stock");
            
            orderItems.Add(new OrderItem(
                product.Id,
                product.Name,
                item.Quantity,
                product.Price
                ));
        }

        var order = new Order(
            Guid.NewGuid(),
            orderRequest.CustomerName,
            orderItems,
            OrderStatus.New,
            DateTimeOffset.UtcNow
        );
        
        var result = await _ordersRepository.Create(order);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateOrderStatusRequest request)
    {
        if (!Enum.IsDefined(typeof(OrderStatus), request.OrderStatus)) 
            return BadRequest($"Order status {request.OrderStatus} is not valid");
        
        var order = await _ordersRepository.GetById(id);
        
        if (order == null) return NotFound($"Order with id {id} not found");
        
        if (order.Status == request.OrderStatus)
            return BadRequest($"Order already has status {request.OrderStatus}");
        
        if (order.Status == OrderStatus.Shipped) 
            return BadRequest($"Order with id {id} is shipped");
        
        if (order.Status == OrderStatus.Cancelled)
            return BadRequest($"Order with id {id} is cancelled");
        
        if (order.Status == OrderStatus.New && request.OrderStatus == OrderStatus.Shipped)
            return BadRequest($"Order with id {id} can have status Processing or Cancelled");
        
        if (order.Status == OrderStatus.Processing && request.OrderStatus == OrderStatus.New)
            return BadRequest($"Order with id {id} can`t have status New");
        
        var result = await _ordersRepository.Update(id, request.OrderStatus);
        
        return Ok(result);
    }
}
