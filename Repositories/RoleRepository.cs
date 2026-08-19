using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Library_Management_System.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public List<Role> GetAll()
        {
            var list = new List<Role>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT RoleId, RoleName FROM Roles ORDER BY RoleId", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(new Role { RoleId = (int)reader["RoleId"], RoleName = (string)reader["RoleName"] });
            }
            return list;
        }

        public int GetRoleIdByName(string roleName)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT RoleId FROM Roles WHERE RoleName=@Name", conn))
            {
                cmd.Parameters.AddWithValue("@Name", roleName);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result == null)
                    throw new System.Exception($"Role '{roleName}' was not found in the database.");
                return (int)result;
            }
        }
    }
}
