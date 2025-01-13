using AwesomePizzaApis.Model;

namespace AwesomePizzaApis.Services;

public interface IOrderService
{
    Order? FindNextOrder(List<Order> orders);

    ICollection<Order> OrganizeOrders(List<Order> orders);
}
