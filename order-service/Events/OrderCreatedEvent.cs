namespace order_service.Events
{
    public record OrderCreatedEvent(Guid OrderId, Guid ProductId, int Quantity);
}
