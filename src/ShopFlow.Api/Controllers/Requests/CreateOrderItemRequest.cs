using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Controllers.Requests;

public sealed record CreateOrderItemRequest(
    [Required] Guid ProductId,
    [Required] int Quantity);