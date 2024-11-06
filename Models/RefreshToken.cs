using Microsoft.AspNetCore.Identity;

namespace Jade.Models
{
    public class RefreshToken
    {
        public int Id { get; set; } // Primary Key
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string UserId { get; set; } // Foreign Key
        public IdentityUser User { get; set; }
    }
}
