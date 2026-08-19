using System.Collections.Generic;
using System.Text.RegularExpressions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    /// <summary>Business logic + validation for managing system users (Admin only screen).</summary>
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetAllUsers() => _userRepository.GetAll();

        public List<User> SearchUsers(string keyword) =>
            string.IsNullOrWhiteSpace(keyword) ? _userRepository.GetAll() : _userRepository.Search(keyword);

        public int CreateUser(User user, string plainPassword)
        {
            Validate(user, plainPassword, isNew: true);

            if (_userRepository.UsernameExists(user.Username))
                throw new ServiceException($"Username '{user.Username}' is already taken.");

            user.Password = AuthService.HashPassword(plainPassword);
            user.Status = string.IsNullOrWhiteSpace(user.Status) ? "Active" : user.Status;
            return _userRepository.Add(user);
        }

        public void UpdateUser(User user)
        {
            Validate(user, plainPassword: null, isNew: false);

            if (_userRepository.UsernameExists(user.Username, user.UserId))
                throw new ServiceException($"Username '{user.Username}' is already taken by another user.");

            _userRepository.Update(user);
        }

        public void DeleteUser(int userId)
        {
            // Business rule: don't allow deleting yourself while logged in.
            if (AuthService.CurrentUser != null && AuthService.CurrentUser.UserId == userId)
                throw new ServiceException("You cannot delete the account you are currently logged in with.");

            _userRepository.Delete(userId);
        }

        public void SetStatus(int userId, bool active) =>
            _userRepository.SetStatus(userId, active ? "Active" : "Inactive");

        private void Validate(User user, string plainPassword, bool isNew)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ServiceException("Full name is required.");
            if (string.IsNullOrWhiteSpace(user.Username) || user.Username.Length < 3)
                throw new ServiceException("Username must be at least 3 characters.");
            if (string.IsNullOrWhiteSpace(user.Email) || !EmailRegex.IsMatch(user.Email))
                throw new ServiceException("Please enter a valid email address.");
            if (user.RoleId <= 0)
                throw new ServiceException("Please select a role.");
            if (isNew && (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 6))
                throw new ServiceException("Password must be at least 6 characters.");
        }
    }
}
