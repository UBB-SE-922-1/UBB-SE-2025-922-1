// <copyright file="CategoryService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Duo.Services
{
    using System;
    using System.Collections.Generic;
    using DuolingoClassLibrary.Entities;
    using Duo.Services.Interfaces;
    using DuolingoClassLibrary.Repositories.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;

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

        public async Task<List<Category>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesAsync();
                return categories;
            }
            catch (Exception ex)
            {
                return new List<Category>();
            }
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(CategoryNameEmptyError);
            }

            try
            {
                var categories = await this._categoryRepository.GetCategoriesAsync();
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

        public async Task<List<string>> GetCategoryNames()
        {
            var categories = await GetAllCategories();
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
