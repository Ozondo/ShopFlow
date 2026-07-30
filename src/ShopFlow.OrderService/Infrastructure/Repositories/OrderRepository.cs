using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShopFlow.Common;
using ShopFlow.Contracts.Product.V1;
using ShopFlow.OrderService.Domain.Orders.Models;
using ShopFlow.OrderService.Infrastructure.Interfaces;

namespace ShopFlow.OrderService.Infrastructure.Repositories;

public class OrderRepository: IOrdersRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _orders = database.GetCollection<Order>("Orders");
    }
    
    public async Task<List<Order>> GetAll()
    {
        return await _orders.Find(_ => true).ToListAsync();
    }

    public async Task<Order?> GetById(Guid id)
    {
        return await _orders.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Order> Create(Order order)
    {
        await _orders.InsertOneAsync(order);
        return order;
    }
    
    public async Task<Order> Update(Order order)
    {
        await _orders.ReplaceOneAsync(x => x.Id == order.Id, order);
        return order;
    }
}