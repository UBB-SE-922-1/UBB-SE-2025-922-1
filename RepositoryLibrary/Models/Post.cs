using System;
using System.Collections.Generic;

namespace Duo.Models
{
    public class Post
    {
        public Post()
        {
            Title = string.Empty;
            Description = string.Empty;
            Hashtags = new List<string>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public int CategoryID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LikeCount { get; set; }

        public string Content { get => Description; set => Description = value; } 
        public string Username { get; set; } 
        public string Date { get; set; } 
        public List<string> Hashtags { get; set; } = new List<string>(); 
    }
}