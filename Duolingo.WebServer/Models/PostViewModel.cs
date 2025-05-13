using System.ComponentModel.DataAnnotations;
using DuolingoClassLibrary.Entities;

namespace Duolingo.WebServer.Models
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Title cannot be longer than 20 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(4000, ErrorMessage = "Content cannot be longer than 4000 characters.")]
        public string Description { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LikeCount { get; set; }

        // For hashtag support
        public List<string> Hashtags { get; set; } = new List<string>();

        // For category selection
        public List<Category> AvailableCategories { get; set; } = new List<Category>();

        // For user display
        public string Username { get; set; }
        public string CategoryName { get; set; }
    }
} 