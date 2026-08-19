using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category GetById(int categoryId);
        List<Category> Search(string keyword);
        bool NameExists(string name, int excludeCategoryId = 0);
        bool IsInUseByBooks(int categoryId);
        int Add(Category category);
        void Update(Category category);
        void Delete(int categoryId);
    }
}
