using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Application.DTOs.Products;
using ShopFlow.Api.Application.Interfaces;

namespace ShopFlow.Api.Controllers;

/// <summary>
/// TODO: реализуй CRUD для товаров.
/// Эндпоинты: GET /api/products, GET /api/products/{id}, POST, PUT, DELETE.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductSevice _productSevice;

    public ProductsController(IProductSevice productSevice)
    {
        _productSevice = productSevice;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _productSevice.GetAll();
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _productSevice.GetById(id);
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
    
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        var result = await _productSevice.Create(request);

        return result.Success ? 
            CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result.Data) 
            : BadRequest(result.Error);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request)
    {
        var result = await _productSevice.Update(id, request);
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productSevice.Delete(id);
        
        return result.Success ? Ok(result.Data) : NotFound($"{result.Error}");
    }
}
