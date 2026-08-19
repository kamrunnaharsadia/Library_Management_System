using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Library_Management_System.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private const string BaseSelect = @"
            SELECT m.MemberId, m.UserId, m.StudentId, m.Department, m.Semester, m.RegistrationDate,
                   u.FullName, u.Email, u.Phone, u.Status
            FROM Members m
            INNER JOIN Users u ON u.UserId = m.UserId";

        public List<Member> GetAll()
        {
            var list = new List<Member>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " ORDER BY u.FullName", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public Member GetById(int memberId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE m.MemberId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", memberId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public Member GetByUserId(int userId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE m.UserId=@UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public List<Member> Find(MemberFilter filter)
        {
            var list = new List<Member>();
            var sql = new StringBuilder(BaseSelect + " WHERE 1=1");

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                sql.Append(" AND (m.StudentId LIKE @Keyword OR u.FullName LIKE @Keyword OR u.Email LIKE @Keyword)");
            if (!string.IsNullOrWhiteSpace(filter.Department))
                sql.Append(" AND m.Department = @Department");
            if (!string.IsNullOrWhiteSpace(filter.Semester))
                sql.Append(" AND m.Semester = @Semester");
            if (!string.IsNullOrWhiteSpace(filter.Status))
                sql.Append(" AND u.Status = @Status");

            sql.Append(" ORDER BY u.FullName");

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql.ToString(), conn))
            {
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                    cmd.Parameters.AddWithValue("@Keyword", "%" + filter.Keyword + "%");
                if (!string.IsNullOrWhiteSpace(filter.Department))
                    cmd.Parameters.AddWithValue("@Department", filter.Department);
                if (!string.IsNullOrWhiteSpace(filter.Semester))
                    cmd.Parameters.AddWithValue("@Semester", filter.Semester);
                if (!string.IsNullOrWhiteSpace(filter.Status))
                    cmd.Parameters.AddWithValue("@Status", filter.Status);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public bool StudentIdExists(string studentId, int excludeMemberId = 0)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Members WHERE StudentId=@Sid AND MemberId<>@ExcludeId", conn))
            {
                cmd.Parameters.AddWithValue("@Sid", studentId);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeMemberId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool HasActiveBorrowings(int memberId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Borrowings WHERE MemberId=@Id AND Status IN ('Active','Overdue')", conn))
            {
                cmd.Parameters.AddWithValue("@Id", memberId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public int Add(Member member)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO Members (UserId, StudentId, Department, Semester, RegistrationDate)
                OUTPUT INSERTED.MemberId
                VALUES (@UserId, @StudentId, @Department, @Semester, @RegistrationDate)", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", member.UserId);
                cmd.Parameters.AddWithValue("@StudentId", member.StudentId);
                cmd.Parameters.AddWithValue("@Department", (object)member.Department ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", (object)member.Semester ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RegistrationDate", member.RegistrationDate == default(DateTime) ? DateTime.Today : member.RegistrationDate);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(Member member)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE Members SET StudentId=@StudentId, Department=@Department, Semester=@Semester
                WHERE MemberId=@MemberId", conn))
            {
                cmd.Parameters.AddWithValue("@StudentId", member.StudentId);
                cmd.Parameters.AddWithValue("@Department", (object)member.Department ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Semester", (object)member.Semester ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MemberId", member.MemberId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int memberId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Members WHERE MemberId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", memberId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static Member Map(IDataRecord r) => new Member
        {
            MemberId = (int)r["MemberId"],
            UserId = (int)r["UserId"],
            StudentId = (string)r["StudentId"],
            Department = r["Department"] as string,
            Semester = r["Semester"] as string,
            RegistrationDate = (DateTime)r["RegistrationDate"],
            FullName = (string)r["FullName"],
            Email = (string)r["Email"],
            Phone = r["Phone"] as string,
            Status = (string)r["Status"]
        };
    }
}
