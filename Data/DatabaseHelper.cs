using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Data
{
    public class DatabaseHelper
    {
        private static readonly string ConnectionString = "Server=localhost\\SQLEXPRESS; database=LibraryManagementDB; Integrated Security=True; Trusted_Connection=True";
            public static SqlConnection GetConnection()
            {
                return new SqlConnection(ConnectionString);
            }
            public static bool TestConnection(out string errorMessage)
            {
                errorMessage = null;
                try
                {
                    using (var conn = GetConnection())
                    {
                        conn.Open();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = "Could not connect to the database. Please check that " +
                                    "SQL Server is running and the connection string is correct.\n\n" +
                                    "Details: " + ex.Message;
                    return false;
                }
            }
        }
}
