using System;
using System.Security.Cryptography;
using System.Text;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public static User CurrentUser { get; private set; }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ServiceException("Please enter your username.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ServiceException("Please enter your password.");

            var user = _userRepository.GetByUsername(username.Trim());
            if (user == null)
                throw new ServiceException("Invalid username or password.");

            if (!user.IsActive)
                throw new ServiceException("Your account is inactive. Please contact the administrator.");

            string hashed = password;
            if (!string.Equals(hashed, user.Password, StringComparison.Ordinal))
                throw new ServiceException("Invalid username or password.");

            CurrentUser = user;
            return user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
        public static string HashPassword(string plainPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
                var sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public void ChangePassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = _userRepository.GetById(userId);
            if (user == null) throw new ServiceException("User not found.");

            if (!string.Equals(currentPassword, user.Password, StringComparison.Ordinal))
                throw new ServiceException("Current password is incorrect.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ServiceException("New password must be at least 6 characters.");

            if (newPassword != confirmPassword)
                throw new ServiceException("New password and confirmation do not match.");

            _userRepository.UpdatePassword(userId, newPassword);
        }
    }
}
