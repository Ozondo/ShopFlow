using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShopFlow.Api.Domain.Orders.Models;

namespace ShopFlow.OrderService.Domain.Orders.Models;

public sealed record Order(
    [property: BsonId]
    [property: BsonRepresentation(BsonType.String)]
    Guid Id,
    string CustomerName,
    IReadOnlyList<OrderItem> Items,
    OrderStatus Status,
    DateTimeOffset CreatedAt);
