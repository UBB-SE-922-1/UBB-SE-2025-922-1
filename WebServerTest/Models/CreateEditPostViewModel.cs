using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DuolingoClassLibrary.Entities;

namespace WebServerTest.Models
{
    public class CreateEditPostViewModel
    {
        public int PostId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(10000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 10000 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Please select a community")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid community")]
        public int CategoryId { get; set; }

        // Used for form input (comma-separated hashtags)
        public string HashtagsString { get; set; }

        // For display and programmatic use
        public List<string> Hashtags { get; set; } = new List<string>();

        // Categories for dropdown
        public List<Category> Categories { get; set; } = new List<Category>();

        // Flag to determine if form is in edit mode
        public bool IsEditing { get; set; } = false;
    }
} 