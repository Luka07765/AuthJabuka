using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Jade.Models
{
    public class Post
    {
        public int Id { get; set; } // Primary Key
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByUserId { get; set; } // Foreign Key
        public IdentityUser CreatedByUser { get; set; }
        public ICollection<Like> Likes { get; set; } // Navigation property
    }
}
