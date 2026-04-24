namespace order_service.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }

    public enum OrderStatus
    {
        Pending,
        Paid,
        Failed
    }
}
