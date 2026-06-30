using Microsoft.AspNetCore.Mvc;
using ShopFlow.Contracts.Product.V1;

namespace ShopFlow.Api.Controllers;

/// <summary>
/// TODO: реализуй CRUD для товаров.
/// Эндпоинты: GET /api/products, GET /api/products/{id}, POST, PUT, DELETE.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(Product.ProductClient productGrpcService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await  productGrpcService.ListProductsAsync(new ListProductsRequest());

        if (result == null) return NotFound("No products were found.");
        
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await productGrpcService.GetProductAsync(new GetProductRequest {Id = id.ToString()});
        
        if (result == null) return NotFound($"Product with id {id} was not found.");
        
        return Ok(result);
    }
    
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        var result =  await productGrpcService.CreateProductAsync(request);

        if (result == null) return BadRequest();

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(UpdateProductRequest request)
    {
        var result = await productGrpcService.UpdateProductAsync(request);
        
        if (result == null) return BadRequest();
        
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await productGrpcService.DeleteProductAsync(new  DeleteProductRequest {Id = id.ToString()});
        
        if (result == null) return BadRequest();
        
        return Ok(result);
    }
}
