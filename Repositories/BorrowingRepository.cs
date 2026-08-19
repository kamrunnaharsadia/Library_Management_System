using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Library_Management_System.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class BorrowingRepository : IBorrowingRepository
    {
        private const string BaseSelect = @"
            SELECT br.BorrowingId, br.BookId, br.MemberId, br.IssuedBy, br.IssueDate, br.DueDate,
                   br.ReturnDate, br.Status,
                   bk.Title AS BookTitle,
                   u.FullName AS MemberName, m.StudentId,
                   ui.FullName AS IssuedByName
            FROM Borrowings br
            INNER JOIN Books bk ON bk.BookId = br.BookId
            INNER JOIN Members m ON m.MemberId = br.MemberId
            INNER JOIN Users u ON u.UserId = m.UserId
            INNER JOIN Users ui ON ui.UserId = br.IssuedBy";

        public List<Borrowing> GetActive()
        {
            var list = new List<Borrowing>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE br.Status IN ('Active','Overdue') ORDER BY br.DueDate", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public List<Borrowing> GetActiveByMember(int memberId)
        {
            var list = new List<Borrowing>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE br.MemberId=@MemberId AND br.Status IN ('Active','Overdue') ORDER BY br.DueDate", conn))
            {
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public List<Borrowing> Find(BorrowingFilter filter)
        {
            var list = new List<Borrowing>();
            var sql = new StringBuilder(BaseSelect + " WHERE 1=1");

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                sql.Append(" AND (u.FullName LIKE @Keyword OR m.StudentId LIKE @Keyword OR bk.Title LIKE @Keyword)");
            if (!string.IsNullOrWhiteSpace(filter.Status))
                sql.Append(" AND br.Status = @Status");

            sql.Append(" ORDER BY br.IssueDate DESC");

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql.ToString(), conn))
            {
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                    cmd.Parameters.AddWithValue("@Keyword", "%" + filter.Keyword + "%");
                if (!string.IsNullOrWhiteSpace(filter.Status))
                    cmd.Parameters.AddWithValue("@Status", filter.Status);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public Borrowing GetById(int borrowingId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE br.BorrowingId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", borrowingId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Add(Borrowing borrowing)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO Borrowings (BookId, MemberId, IssuedBy, IssueDate, DueDate, ReturnDate, Status)
                OUTPUT INSERTED.BorrowingId
                VALUES (@BookId, @MemberId, @IssuedBy, @IssueDate, @DueDate, NULL, 'Active')", conn))
            {
                cmd.Parameters.AddWithValue("@BookId", borrowing.BookId);
                cmd.Parameters.AddWithValue("@MemberId", borrowing.MemberId);
                cmd.Parameters.AddWithValue("@IssuedBy", borrowing.IssuedBy);
                cmd.Parameters.AddWithValue("@IssueDate", borrowing.IssueDate);
                cmd.Parameters.AddWithValue("@DueDate", borrowing.DueDate);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void MarkReturned(int borrowingId, DateTime returnDate)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "UPDATE Borrowings SET ReturnDate=@ReturnDate, Status='Returned' WHERE BorrowingId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                cmd.Parameters.AddWithValue("@Id", borrowingId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStatus(int borrowingId, string status)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("UPDATE Borrowings SET Status=@Status WHERE BorrowingId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Id", borrowingId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Borrowing> GetOverdueActive()
        {
            var list = new List<Borrowing>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                BaseSelect + " WHERE br.Status IN ('Active','Overdue') AND br.DueDate < CAST(GETDATE() AS DATE)", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        private static Borrowing Map(IDataRecord r) => new Borrowing
        {
            BorrowingId = (int)r["BorrowingId"],
            BookId = (int)r["BookId"],
            MemberId = (int)r["MemberId"],
            IssuedBy = (int)r["IssuedBy"],
            IssueDate = (DateTime)r["IssueDate"],
            DueDate = (DateTime)r["DueDate"],
            ReturnDate = r["ReturnDate"] as DateTime?,
            Status = (string)r["Status"],
            BookTitle = (string)r["BookTitle"],
            MemberName = (string)r["MemberName"],
            StudentId = (string)r["StudentId"],
            IssuedByName = (string)r["IssuedByName"]
        };
    }
}
