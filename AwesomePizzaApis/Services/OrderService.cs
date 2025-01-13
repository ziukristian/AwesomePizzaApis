using AwesomePizzaApis.Model;

namespace AwesomePizzaApis.Services;

public class OrderService : IOrderService
{
    public Order? FindNextOrder(List<Order> orders)
    {
        if (orders == null || orders.Count == 0)
        {
            return null;
        }

        return orders
            .FindAll(o => o.Status == OrderStatus.Created)
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefault();
    }

    public ICollection<Order> OrganizeOrders(List<Order> orders)
    {
        if (orders == null || orders.Count == 0)
        {
            return [];
        }

        return [.. orders.OrderBy(x => x.CreatedAt)];
    }
}
