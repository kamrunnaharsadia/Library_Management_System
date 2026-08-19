using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Library_Management_System.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string BaseSelect = @"
            SELECT u.UserId, u.FullName, u.Username, u.Email, u.Password, u.Phone,
                   u.RoleId, r.RoleName, u.Status, u.CreatedAt
            FROM Users u
            INNER JOIN Roles r ON r.RoleId = u.RoleId";

        public User GetByUsername(string username)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE u.Username = @Username", conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? Map(reader) : null;
                }
            }
        }

        public User GetById(int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE u.UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() ? Map(reader) : null;
                }
            }
        }

        public List<User> GetAll()
        {
            var list = new List<User>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " ORDER BY u.FullName", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(Map(reader));
                }
            }
            return list;
        }

        public List<User> Search(string keyword)
        {
            var list = new List<User>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect +
                " WHERE u.FullName LIKE @Keyword OR u.Username LIKE @Keyword OR u.Email LIKE @Keyword " +
                " ORDER BY u.FullName", conn))
            {
                cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(Map(reader));
                }
            }
            return list;
        }

        public bool UsernameExists(string username, int excludeUserId = 0)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Users WHERE Username = @Username AND UserId <> @ExcludeId", conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeUserId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public int Add(User user)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO Users (FullName, Username, Email, Password, Phone, RoleId, Status, CreatedAt)
                OUTPUT INSERTED.UserId
                VALUES (@FullName, @Username, @Email, @Password, @Phone, @RoleId, @Status, GETDATE())", conn))
            {
                AddCommonParams(cmd, user);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(User user)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE Users SET FullName=@FullName, Email=@Email, Phone=@Phone,
                                  RoleId=@RoleId, Status=@Status
                WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@FullName", user.FullName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@Status", user.Status);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void UpdatePassword(int userId, string newPasswordHash)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("UPDATE Users SET Password=@Password WHERE UserId=@UserId", conn))
            {
                cmd.Parameters.AddWithValue("@Password", newPasswordHash);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Users WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void SetStatus(int userId, string status)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("UPDATE Users SET Status=@Status WHERE UserId=@UserId", conn))
            {
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static void AddCommonParams(SqlCommand cmd, User user)
        {
            cmd.Parameters.AddWithValue("@FullName", user.FullName);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
            cmd.Parameters.AddWithValue("@Status", user.Status ?? "Active");
        }

        private static User Map(IDataRecord r) => new User
        {
            UserId = (int)r["UserId"],
            FullName = (string)r["FullName"],
            Username = (string)r["Username"],
            Email = (string)r["Email"],
            Password = (string)r["Password"],
            Phone = r["Phone"] as string,
            RoleId = (int)r["RoleId"],
            RoleName = (string)r["RoleName"],
            Status = (string)r["Status"],
            CreatedAt = (DateTime)r["CreatedAt"]
        };
    }
}
