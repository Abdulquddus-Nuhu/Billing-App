using System.ComponentModel.DataAnnotations;

namespace BillingApp.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }

        public Transaction()
        {
            Id = Guid.NewGuid();
        }
    }
}
