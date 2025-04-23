using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Server.Entities
{
    public class MyCourse
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public double CompletionPercentage { get; set; }
        public DateTime EnrollmentDate { get; set; }

        // Added formatted property
        public string FormattedCompletion => $"{CompletionPercentage * 100:F0}%";
    }
}