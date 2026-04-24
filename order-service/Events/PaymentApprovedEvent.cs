namespace order_service.Events
{
    public record PaymentApprovedEvent(Guid OrderId);
}
