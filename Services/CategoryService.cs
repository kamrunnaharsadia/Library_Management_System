using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> GetAllCategories() => _categoryRepository.GetAll();

        public List<Category> SearchCategories(string keyword) =>
            string.IsNullOrWhiteSpace(keyword) ? _categoryRepository.GetAll() : _categoryRepository.Search(keyword);

        public int AddCategory(Category category)
        {
            Validate(category);
            if (_categoryRepository.NameExists(category.CategoryName))
                throw new ServiceException($"A category named '{category.CategoryName}' already exists.");
            return _categoryRepository.Add(category);
        }

        public void UpdateCategory(Category category)
        {
            Validate(category);
            if (_categoryRepository.NameExists(category.CategoryName, category.CategoryId))
                throw new ServiceException($"A category named '{category.CategoryName}' already exists.");
            _categoryRepository.Update(category);
        }

        public void DeleteCategory(int categoryId)
        {
            // Business rule: a category currently used by books cannot be deleted -
            // this keeps every Book pointing at a valid category (referential integrity).
            if (_categoryRepository.IsInUseByBooks(categoryId))
                throw new ServiceException("This category cannot be deleted because it is used by one or more books. " +
                                            "Reassign those books to a different category first.");
            _categoryRepository.Delete(categoryId);
        }

        private void Validate(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ServiceException("Category name is required.");
            if (category.CategoryName.Length > 100)
                throw new ServiceException("Category name is too long (max 100 characters).");
        }
    }
}
