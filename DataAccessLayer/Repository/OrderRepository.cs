namespace DataAccessLayer.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orderCollection;
    private readonly string collectionName = "Orders";
    public OrderRepository(IMongoDatabase mongoDatabase)
    {
       _orderCollection = mongoDatabase.GetCollection<Order>(collectionName);
    }

    public async Task<Order?> CreateOrderAsync(Order order)
    {
        order.OrderID = Guid.NewGuid();
        order._id = order.OrderID;

        foreach (var item in order.Items)
        {
            item._id = Guid.NewGuid();
        }

        await _orderCollection.InsertOneAsync(order);
        return order;
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        DeleteResult result = await _orderCollection.DeleteOneAsync(o => o.OrderID == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderCollection.Find(_ => true).ToListAsync();

    public async Task<IEnumerable<Order>> GetOrdersWithConditionAsync(FilterDefinition<Order> filter)
    {
        return await _orderCollection.Find(filter).ToListAsync();
    }

    public async Task<Order?> GetSingleOrderAsync(FilterDefinition<Order> filter)
    {
        return await _orderCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(o => o.OrderID, order.OrderID);
        Order existingOrder = await (await _orderCollection.FindAsync(filter)).FirstOrDefaultAsync();

        order._id = existingOrder._id;

        var result = await _orderCollection.ReplaceOneAsync(o => o.OrderID == order.OrderID, order);

        if (result.MatchedCount == 0)
        {
            throw new ArgumentNullException(nameof(order), "Order not found");
        }

        return order;
    }
}
