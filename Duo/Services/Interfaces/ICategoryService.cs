// <copyright file="ICategoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Duo.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Server.Entities;

    public interface ICategoryService
    {
        List<Category> GetAllCategories();
        Category GetCategoryByName(string name);
    }
}
