using Microsoft.AspNetCore.Identity;

namespace fullstackecommercewebapp.Models
{
    public partial class AspNetRoles : IdentityRole<int>
    {
        public AspNetRoles()
        {
        }

        public override int Id { get; set; }

    }
}
