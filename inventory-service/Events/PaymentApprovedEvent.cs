namespace inventory_service.Events
{
    public record PaymentApprovedEvent(Guid OrderId, Guid ProductId, int Quantity);
}
