namespace IntroductionASPNET.Services.OrderServices
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrders();
        Task<Result<Order>> GetOrdersByID(string OrderNumber);
        string GetIDOrderValue();
        Task<Result<Order>> AddOrder(Order order);
        Task<Result<Order>> UpdateOrder(Order order);
        Task<Result<Order>> DeleteOrder(string OrderNumber);
    }
}
