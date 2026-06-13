using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Application.DTOs.Orders;
using ShopFlow.Api.Application.Interfaces;

namespace ShopFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _orderService.GetAll();
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetById(id);
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest orderRequest)
    {
        var result = await _orderService.Create(orderRequest);
        
        return result.Success ? 
            CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data)
            : BadRequest($"{result.Error}");
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateOrderStatusRequest request)
    {
        var result = await _orderService.Update(id, request);
        return result.Success ?  Ok(result.Data) : BadRequest($"{result.Error}");
    }
}
