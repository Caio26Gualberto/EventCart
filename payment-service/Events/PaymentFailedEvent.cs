namespace payment_service.Events
{
    public record PaymentFailedEvent(Guid OrderId);
}
