using Grpc.Core;
using MediatR;
using ShopFlow.Contracts.Order.V1;
using ShopFlow.OrderService.Usecase.CreateOrder;
using ShopFlow.OrderService.Usecase.GetAll;
using ShopFlow.OrderService.Usecase.GetOrder;
using ShopFlow.OrderService.Usecase.UpdateOrder;
using Order = ShopFlow.OrderService.Domain.Orders.Models.Order;
using OrderStatus = ShopFlow.Api.Domain.Orders.Models.OrderStatus;
using OrderGRPC = ShopFlow.Contracts.Order.V1.Order;

namespace ShopFlow.OrderService.Endpoints;

public class OrderGrpcService(IMediator mediator): OrderGRPC.OrderBase
{
    public override async Task<GetAllOrderResponse> GetAll(GetAllOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new GetAllQuery());
        
        var response = new GetAllOrderResponse();
        response.Orders.AddRange(OrderMapper.Map(result));
        
        return response;
    }

    public override async Task<GetOrderResponse?> GetOrder(GetOrderRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new GetOrderQuery(Guid.Parse(request.Id)));
        return OrderMapper.Map(result);
    }

    public override async Task<GetOrderResponse?> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var command = new CreateOrderCommand(
            request.CustomerName,
            request.Items
                .Select(x => new CreateOrderItem(
                    Guid.Parse(x.ProductId),
                    x.Quantity))
                .ToList());
        
        var result = await mediator.Send(command);
        return OrderMapper.Map(result);
    }

    public override async Task<GetOrderResponse?> UpdateOrderStatus(UpdateStatusRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(new UpdateOrderCommand(Guid.Parse(request.Id), (OrderStatus)request.OrderStatus));
        
        return OrderMapper.Map(result);
    }
}