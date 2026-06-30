using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Controllers.Requests;
using ShopFlow.Contracts.Order.V1;

namespace ShopFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController(Order.OrderClient orderGrpcService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await orderGrpcService.GetAllAsync( new GetAllOrderRequest());
        
        if (result == null) return NotFound();
        
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await orderGrpcService.GetOrderAsync(new GetOrderRequest{Id = id.ToString()});
        
        if (result == null) return NotFound();
        
        return Ok(result);
    }
    
    [HttpPost]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> Create(CreateOrderRequestController orderRequest)
    {
        var grpcRequest = new CreateOrderRequest
        {
            CustomerName = orderRequest.CustomerName
        };

        grpcRequest.Items.AddRange(
            orderRequest.Items.Select(x => new CreateItemRequest
            {
                ProductId = x.ProductId.ToString(),
                Quantity = x.Quantity
            })
        );
        
        var result = await orderGrpcService.CreateOrderAsync(grpcRequest);
        
        if (result == null) return NotFound();

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    
    [HttpPatch]
    public async Task<IActionResult> Update(UpdateStatusRequest updateRequest)
    {
        var result = await orderGrpcService.UpdateOrderStatusAsync(updateRequest);
        
        if (result == null) return NotFound();

        return Ok(result);
    }
}
