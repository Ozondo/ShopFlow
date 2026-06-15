using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Application.DTOs.Orders;

public sealed record CreateOrderItemRequest(
    [Required] Guid ProductId,
    [Required] int Quantity);