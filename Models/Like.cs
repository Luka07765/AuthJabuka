using Microsoft.AspNetCore.Identity;

namespace Jade.Models
{
    public class Like
    {
        public int Id { get; set; } // Primary Key
        public int PostId { get; set; } // Foreign Key
        public Post Post { get; set; }
        public string UserId { get; set; } // Foreign Key
        public IdentityUser User { get; set; }
    }
}
