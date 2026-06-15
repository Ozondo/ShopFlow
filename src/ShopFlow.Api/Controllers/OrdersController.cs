using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Application.DTOs.Orders;
using ShopFlow.Api.Application.Interfaces;

namespace ShopFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await orderService.GetAll();
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await orderService.GetById(id);
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
    
    [HttpPost]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> Create(CreateOrderRequest orderRequest)
    {
        var result = await orderService.Create(orderRequest);
        
        return result.Success ? 
            CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data)
            : BadRequest($"{result.Error}");
    }
    
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateOrderStatusRequest request)
    {
        var result = await orderService.Update(id, request);
        return result.Success ?  Ok(result.Data) : BadRequest($"{result.Error}");
    }
}
