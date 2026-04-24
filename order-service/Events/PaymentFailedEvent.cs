namespace order_service.Events
{
    public record PaymentFailedEvent(Guid OrderId);
}
