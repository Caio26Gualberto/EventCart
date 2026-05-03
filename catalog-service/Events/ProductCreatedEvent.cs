namespace catalog_service.Events
{
    public record ProductCreatedEvent(Guid ProductId, int InitialQuantity);
}
