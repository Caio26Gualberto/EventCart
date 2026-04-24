using Microsoft.EntityFrameworkCore;
using payment_service.Entities;

namespace payment_service.Context
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
    }
}
