namespace OrderProcessing.Infrastructure.Messaging;

public record OrderMessage(Guid Id, string Client, decimal Value, DateTime OrderDate);
