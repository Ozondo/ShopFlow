using Microsoft.AspNetCore.Mvc;
using ShopFlow.Api.Application.DTOs.Products;
using ShopFlow.Api.Application.Interfaces;
using ShopFlow.Api.Domain.Products.Models;

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
    // TODO: внедри IProductRepository через конструктор
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    // TODO: добавь action-методы

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productRepository.GetAll();
        return Ok(products);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productRepository.GetById(id);
        
        if (product == null)  return NotFound($"Product by id = {id} not found");
        
        return Ok(product);
    }
    
    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        if (request.Stock < 1) return BadRequest($"Stock must be greater than or equal to 1.");
        if (request.Price <= 0) return BadRequest($"Price must be greater than 0.");
        if (string.IsNullOrEmpty(request.Name)) return BadRequest($"Product name is required.");
        if (string.IsNullOrEmpty(request.Category)) return BadRequest($"Product category is required.");
        
        var product = new Product(
            Guid.NewGuid(),
            request.Name,
            request.Category,
            request.Price,
            request.Stock
            );
        
        var result = await _productRepository.Create(product);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request)
    {
        if (request.Stock < 1) return BadRequest($"Stock must be greater than or equal to 1.");
        if (request.Price <= 0) return BadRequest($"Price must be greater than 0.");
        if (string.IsNullOrEmpty(request.Name)) return BadRequest($"Product name is required.");
        if (string.IsNullOrEmpty(request.Category)) return BadRequest($"Product category is required.");
        
        
        var product = new Product(
            id,
            request.Name,
            request.Category,
            request.Price,
            request.Stock
        );
        
        var result = await _productRepository.Update(id, product);
        
        if(result == null) return NotFound($"Product with id = {id} not found");
        
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productRepository.Delete(id);
        
        if(result == null) return NotFound($"Product with id = {id} not found");
        
        return Ok(result);
    }
}
