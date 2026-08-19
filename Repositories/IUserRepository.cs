using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
        User GetById(int userId);
        List<User> GetAll();
        List<User> Search(string keyword);
        bool UsernameExists(string username, int excludeUserId = 0);
        int Add(User user);
        void Update(User user);
        void UpdatePassword(int userId, string newPasswordHash);
        void Delete(int userId);
        void SetStatus(int userId, string status);
    }
}
