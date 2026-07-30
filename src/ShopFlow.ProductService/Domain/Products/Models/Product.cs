using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShopFlow.ProductService.Domain.Products.Models;

public sealed record Product(
    [property: BsonId]
    [property: BsonRepresentation(BsonType.String)]
    Guid Id,
    string Name,
    string Category,
    decimal Price,
    int Stock);
