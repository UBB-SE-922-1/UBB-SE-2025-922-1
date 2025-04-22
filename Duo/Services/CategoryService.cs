// <copyright file="CategoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Duo.Services
{
    using System;
    using System.Collections.Generic;
    using Server.Entities;
    using Duo.Repositories;
    using Duo.Repositories.Interfaces;
    using Duo.Services.Interfaces;
    using Server.Repositories.Interfaces;
    using System.Linq;

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        // Constants for error messages
        private const string ErrorFetchingCategories = "Error fetching categories: {0}";
        private const string ErrorFetchingCategory = "Error fetching category: {0}";
        private const string CategoryNotFound = "Category not found";
        private const string CategoryNameEmptyError = "Category name cannot be empty";
        private const string CategoriesListNull = "Categories list is null";

        // Constants for default values
        private const string EmptyString = "";

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public List<Category> GetAllCategories()
        {
            try
            {
                return _categoryRepository.GetCategories();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(ErrorFetchingCategories, ex.Message));
                return new List<Category>();
            }
        }

        public Category GetCategoryByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(CategoryNameEmptyError);
            }

            try
            {
                var categories = this._categoryRepository.GetCategories();
                if (categories == null)
                {
                    Console.WriteLine(CategoriesListNull);
                    return null;
                }

                Category category = categories.FirstOrDefault(c => c.Name == name);

                if (category == null)
                {
                    Console.WriteLine($"Category with name '{name}' not found");
                    return null;
                }

                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(ErrorFetchingCategory, ex.Message));
                return null;
            }
        }

        public List<string> GetCategoryNames()
        {
            var categories = GetAllCategories();
            if (categories == null)
            {
                return new List<string>();
            }
            return categories.ConvertAll(c => c?.Name ?? string.Empty);
        }

        public bool IsValidCategoryName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var category = GetCategoryByName(name);
            return category != null;
        }
    }
}
