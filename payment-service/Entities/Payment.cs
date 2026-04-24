namespace payment_service.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum PaymentStatus
    {
        Pending,
        Approved,
        Failed
    }
}
