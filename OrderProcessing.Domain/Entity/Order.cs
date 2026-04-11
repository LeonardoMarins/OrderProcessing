namespace OrderProcessing.Domain.Entity;

public class Order
{
    public Guid Id { get; private set; }
    public string Client { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public DateTime OrderDate { get; private set; }
}