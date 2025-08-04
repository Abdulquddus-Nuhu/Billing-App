using BillingApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace BillingApp.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            Id = Guid.NewGuid();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public RoleType Role { get; set; }
        public decimal Balance { get; set; } = 20_000;
    }
}
