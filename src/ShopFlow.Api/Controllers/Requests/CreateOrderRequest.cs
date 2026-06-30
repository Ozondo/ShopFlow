using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Api.Controllers.Requests;

public sealed record CreateOrderRequestController(
    [Required] string CustomerName,
    [Required] IReadOnlyList<CreateOrderItemRequest> Items);