﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DuolingoClassLibrary.Entities;

public class Course
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalNumberOfLessons { get; set; }

}