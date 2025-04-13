﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Models
{
    public class Quiz
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double AccuracyPercentage { get; set; }
        public DateTime CompletionDate { get; set; }

        // Added formatted property
        public string FormattedAccuracy => $"{AccuracyPercentage * 100:F0}%";
    }
}