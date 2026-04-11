namespace OrderProcessing.Domain.Entity;

public class Order
{
    public Guid Id { get; private set; }
    public string Client { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public DateTime OrderDate { get; private set; }

    private Order() { }

    public static Order Create(string client, decimal value)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            Client = client,
            Value = value,
            OrderDate = DateTime.UtcNow
        };
    }

    public static Order Restore(Guid id, string client, decimal value, DateTime orderDate)
    {
        return new Order
        {
            Id = id,
            Client = client,
            Value = value,
            OrderDate = orderDate
        };
    }
}