using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Controllers.Requests;

public sealed record CreateOrderRequest(
    [Required] string CustomerName,
    [Required] IReadOnlyList<CreateOrderItemRequest> Items);