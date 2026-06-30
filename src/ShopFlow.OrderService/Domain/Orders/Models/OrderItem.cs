using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShopFlow.Api.Domain.Orders.Models;

public sealed record OrderItem(
    [property: BsonId]
    [property: BsonRepresentation(BsonType.String)]
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
