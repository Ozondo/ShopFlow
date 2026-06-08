using Microsoft.AspNetCore.Mvc;

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
    // TODO: добавь action-методы
}
