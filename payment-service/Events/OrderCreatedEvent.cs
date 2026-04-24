namespace payment_service.Events
{
    public record OrderCreatedEvent(Guid OrderId, Guid ProductId, int Quantity);
}
