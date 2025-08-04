using Microsoft.AspNetCore.Identity;

namespace BillingApp.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {
            Id = Guid.NewGuid();
        }

        public Role(string roleName)
        {
            Name = roleName;
        }

    }
}
