namespace inventory_service.Events
{
    public record ProductCreatedEvent(Guid ProductId, int InitialQuantity);
}
