﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Models
{
    public class MyCourse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public double CompletionPercentage { get; set; }
        public DateTime EnrollmentDate { get; set; }

        // Added formatted property
        public string FormattedCompletion => $"{CompletionPercentage * 100:F0}%";
    }
}