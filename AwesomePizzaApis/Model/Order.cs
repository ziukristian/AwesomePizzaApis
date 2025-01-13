namespace AwesomePizzaApis.Model;

public class Order
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public required string PizzaType { get; set; }
    public required string CustomerEmail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum OrderStatus
{
    Created,
    InProgress,
    Finished,
    Cancelled,
}
