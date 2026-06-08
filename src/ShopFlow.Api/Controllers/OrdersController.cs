using Microsoft.AspNetCore.Mvc;

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
    // TODO: добавь action-методы и бизнес-правила (валидация stock, переходы статусов)
}
