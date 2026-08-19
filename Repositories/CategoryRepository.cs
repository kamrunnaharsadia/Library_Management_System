using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Library_Management_System.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAll()
        {
            var list = new List<Category>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT CategoryId, CategoryName, Description FROM Categories ORDER BY CategoryName", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public Category GetById(int categoryId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT CategoryId, CategoryName, Description FROM Categories WHERE CategoryId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", categoryId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public List<Category> Search(string keyword)
        {
            var list = new List<Category>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT CategoryId, CategoryName, Description FROM Categories WHERE CategoryName LIKE @Keyword ORDER BY CategoryName", conn))
            {
                cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public bool NameExists(string name, int excludeCategoryId = 0)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Categories WHERE CategoryName=@Name AND CategoryId<>@ExcludeId", conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeCategoryId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool IsInUseByBooks(int categoryId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM Books WHERE CategoryId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", categoryId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public int Add(Category category)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "INSERT INTO Categories (CategoryName, Description) OUTPUT INSERTED.CategoryId VALUES (@Name, @Description)", conn))
            {
                cmd.Parameters.AddWithValue("@Name", category.CategoryName);
                cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? DBNull.Value);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(Category category)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "UPDATE Categories SET CategoryName=@Name, Description=@Description WHERE CategoryId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Name", category.CategoryName);
                cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Id", category.CategoryId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int categoryId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Categories WHERE CategoryId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", categoryId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static Category Map(IDataRecord r) => new Category
        {
            CategoryId = (int)r["CategoryId"],
            CategoryName = (string)r["CategoryName"],
            Description = r["Description"] as string
        };
    }
}
