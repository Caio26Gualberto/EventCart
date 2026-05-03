namespace order_service.Requests
{
    public class CreateOrderRequest
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
