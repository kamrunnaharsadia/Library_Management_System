using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Library_Management_System.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class FineRepository : IFineRepository
    {
        private const string BaseSelect = @"
            SELECT f.FineId, f.BorrowingId, f.Amount, f.Reason, f.PaidStatus, f.CreatedAt,
                   u.FullName AS StudentName, m.StudentId, bk.Title AS BookTitle
            FROM Fines f
            INNER JOIN Borrowings br ON br.BorrowingId = f.BorrowingId
            INNER JOIN Members m ON m.MemberId = br.MemberId
            INNER JOIN Users u ON u.UserId = m.UserId
            INNER JOIN Books bk ON bk.BookId = br.BookId";

        public List<Fine> GetAll()
        {
            var list = new List<Fine>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " ORDER BY f.CreatedAt DESC", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public List<Fine> GetByMember(int memberId)
        {
            var list = new List<Fine>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE m.MemberId=@MemberId ORDER BY f.CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public List<Fine> Find(FineFilter filter)
        {
            var list = new List<Fine>();
            var sql = new StringBuilder(BaseSelect + " WHERE 1=1");

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                sql.Append(" AND (u.FullName LIKE @Keyword OR m.StudentId LIKE @Keyword)");
            if (!string.IsNullOrWhiteSpace(filter.PaidStatus))
                sql.Append(" AND f.PaidStatus = @PaidStatus");

            sql.Append(" ORDER BY f.CreatedAt DESC");

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql.ToString(), conn))
            {
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                    cmd.Parameters.AddWithValue("@Keyword", "%" + filter.Keyword + "%");
                if (!string.IsNullOrWhiteSpace(filter.PaidStatus))
                    cmd.Parameters.AddWithValue("@PaidStatus", filter.PaidStatus);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public Fine GetByBorrowingId(int borrowingId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE f.BorrowingId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", borrowingId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Add(Fine fine)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO Fines (BorrowingId, Amount, Reason, PaidStatus, CreatedAt)
                OUTPUT INSERTED.FineId
                VALUES (@BorrowingId, @Amount, @Reason, 'Unpaid', GETDATE())", conn))
            {
                cmd.Parameters.AddWithValue("@BorrowingId", fine.BorrowingId);
                cmd.Parameters.AddWithValue("@Amount", fine.Amount);
                cmd.Parameters.AddWithValue("@Reason", (object)fine.Reason ?? DBNull.Value);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void MarkPaid(int fineId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("UPDATE Fines SET PaidStatus='Paid' WHERE FineId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", fineId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public decimal GetTotalUnpaid()
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT ISNULL(SUM(Amount),0) FROM Fines WHERE PaidStatus='Unpaid'", conn))
            {
                conn.Open();
                return (decimal)cmd.ExecuteScalar();
            }
        }

        public decimal GetTotalUnpaidByMember(int memberId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(f.Amount),0)
                FROM Fines f
                INNER JOIN Borrowings br ON br.BorrowingId = f.BorrowingId
                WHERE br.MemberId=@MemberId AND f.PaidStatus='Unpaid'", conn))
            {
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                conn.Open();
                return (decimal)cmd.ExecuteScalar();
            }
        }

        private static Fine Map(IDataRecord r) => new Fine
        {
            FineId = (int)r["FineId"],
            BorrowingId = (int)r["BorrowingId"],
            Amount = (decimal)r["Amount"],
            Reason = r["Reason"] as string,
            PaidStatus = (string)r["PaidStatus"],
            CreatedAt = (DateTime)r["CreatedAt"],
            StudentName = (string)r["StudentName"],
            StudentId = (string)r["StudentId"],
            BookTitle = (string)r["BookTitle"]
        };
    }
}
