using Grpc.Core;
using MediatR;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.ProductService.Usecase.CreateProduct;
using ShopFlow.ProductService.Usecase.DeleteProduct;
using ShopFlow.ProductService.Usecase.GetByIds;
using ShopFlow.ProductService.Usecase.GetProduct;
using ShopFlow.ProductService.Usecase.ListProducts;
using ShopFlow.ProductService.Usecase.UpdateProduct;
using ProductGRPC = ShopFlow.Contracts.Product.V1.Product;

namespace ShopFlow.ProductService.Endpoints;

public class ProductGrpcService(IMediator mediator): ProductGRPC.ProductBase
{
    public override async Task<ProductResponse?> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new GetProductQuery(Id: Guid.Parse(request.Id)));
        
        return ProductMapper.Map(result);
    }

    public override async Task<ListProductsResponse?> ListProducts(ListProductsRequest request,
        ServerCallContext context)
    {
        var products = await mediator.Send(new ListProductsQuery());
        var response = new ListProductsResponse();
        response.Products.AddRange(ProductMapper.Map(products));

        return response;
    }

    public override async Task<ProductResponse?> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new CreateProductCommand(
            Name: request.Name,
            Category: request.Category,
            Price: Decimal.Parse(request.Price),
            Stock: request.Stock)
        );
        
        return ProductMapper.Map(result);
    }

    public override async Task<ProductResponse?> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new UpdateProductCommand(
            Guid.Parse(request.Id),
            request.Name,
            request.Category,
            decimal.Parse(request.Price),
            request.Stock)
        );

        return ProductMapper.Map(result);
    }

    public override async Task<ProductResponse?> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new DeleteProductCommand(Guid.Parse(request.Id)));
        return ProductMapper.Map(result);
    }
    
    public override async Task<GetByIdsResponse?> GetByIds(GetByIdsRequest request, ServerCallContext context)
    {
        var ids = request.Ids.Select(x => Guid.Parse(x)).ToList();
        var products = await mediator.Send(new GetByIdsQuery(ids));
        
        var response = new GetByIdsResponse();
        response.Products.AddRange(ProductMapper.Map(products));

        return response;
    }
}