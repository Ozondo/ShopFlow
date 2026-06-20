using Grpc.Core;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.ProductService.Domain.Products.Models;
using ShopFlow.ProductService.Infrastructure.Interfaces;
using Product = ShopFlow.Contracts.Product.V1.Product;

namespace ShopFlow.ProductService.Endpoints;

public class ProductGrpcService(IProductRepository productRepository): Product.ProductBase
{
    public override async Task<ProductResponse?> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var result = await productRepository.GetById(Guid.Parse(request.Id));
        
        return ProductMapper.Map(result);
    }

    public override async Task<ListProductsResponse?> ListProducts(ListProductsRequest request,
        ServerCallContext context)
    {
        var products = await productRepository.GetAll();

        var response = new ListProductsResponse();

        response.Products.AddRange(ProductMapper.Map(products));

        return response;
    }

    public override async Task<ProductResponse?> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var product = new Domain.Products.Models.Product(
            Guid.NewGuid(),
            request.Name,
            request.Category, 
            decimal.Parse(request.Price),
            request.Stock
        );
        
        var result = await productRepository.Create(product);
        
        return ProductMapper.Map(result);
    }

    public override async Task<ProductResponse?> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        var product = new Domain.Products.Models.Product(
            Guid.NewGuid(),
            request.Name,
            request.Category, 
            decimal.Parse(request.Price),
            request.Stock
        );
        
        var result = await productRepository.Update(product);

        return ProductMapper.Map(result);
    }

    public override async Task<ProductResponse?> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        var result = await productRepository.Delete(Guid.Parse(request.Id));
        return ProductMapper.Map(result);
    }
    
    public override async Task<GetByIdsResponse?> GetByIds(GetByIdsRequest request, ServerCallContext context)
    {
        var ids = request.Ids.Select(x => Guid.Parse(x)).ToList();
        var products = await productRepository.GetByIds(ids);
        
        var response = new GetByIdsResponse();
        response.Products.AddRange(ProductMapper.Map(products));

        return response;
    }
}