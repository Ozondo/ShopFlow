using System.Globalization;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.ProductService.Domain.Products.Models;
using Product = ShopFlow.ProductService.Domain.Products.Models.Product;

namespace ShopFlow.ProductService.Endpoints;

public static class ProductMapper
{
    public static ProductResponse? Map(Product? product)
    {
        if (product == null) return null;
        
        return new ProductResponse
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Category =  product.Category,
            Price = product.Price.ToString(CultureInfo.InvariantCulture),
            Stock = product.Stock,
        };
    }
    
    public static List<ProductResponse>? Map(IEnumerable<Product>? products)
    {
        if (products == null) return null;
        
        return products
            .Select(Map)
            .Cast<ProductResponse>()
            .ToList();
    }
}