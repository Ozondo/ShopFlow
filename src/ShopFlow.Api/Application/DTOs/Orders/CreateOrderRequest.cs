using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Application.DTOs.Orders;

public sealed record CreateOrderRequest(
    [Required] string CustomerName,
    [Required] IReadOnlyList<CreateOrderItemRequest> Items);