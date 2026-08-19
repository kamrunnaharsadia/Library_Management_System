using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Library_Management_System.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class BookRepository : IBookRepository
    {
        private const string BaseSelect = @"
            SELECT b.BookId, b.ISBN, b.Title, b.Author, b.Publisher, b.CategoryId,
                   c.CategoryName, b.PublicationYear, b.Quantity, b.AvailableQuantity,
                   b.ShelfLocation, b.Status, b.CreatedAt
            FROM Books b
            INNER JOIN Categories c ON c.CategoryId = b.CategoryId";

        public List<Book> GetAll()
        {
            var list = new List<Book>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " ORDER BY b.Title", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public Book GetById(int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(BaseSelect + " WHERE b.BookId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", bookId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        /// <summary>Builds a single parameterized query from whichever
        /// filter fields were supplied - never string-concatenates user input.</summary>
        public List<Book> Find(BookFilter filter)
        {
            var list = new List<Book>();
            var sql = new StringBuilder(BaseSelect + " WHERE 1=1");

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                sql.Append(" AND (b.ISBN LIKE @Keyword OR b.Title LIKE @Keyword OR b.Author LIKE @Keyword)");
            if (filter.CategoryId.HasValue)
                sql.Append(" AND b.CategoryId = @CategoryId");
            if (filter.AvailableOnly == true)
                sql.Append(" AND b.AvailableQuantity > 0");
            if (!string.IsNullOrWhiteSpace(filter.Status))
                sql.Append(" AND b.Status = @Status");

            sql.Append(" ORDER BY b.Title");

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql.ToString(), conn))
            {
                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                    cmd.Parameters.AddWithValue("@Keyword", "%" + filter.Keyword + "%");
                if (filter.CategoryId.HasValue)
                    cmd.Parameters.AddWithValue("@CategoryId", filter.CategoryId.Value);
                if (!string.IsNullOrWhiteSpace(filter.Status))
                    cmd.Parameters.AddWithValue("@Status", filter.Status);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) list.Add(Map(reader));
            }
            return list;
        }

        public bool IsbnExists(string isbn, int excludeBookId = 0)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM Books WHERE ISBN=@Isbn AND BookId<>@ExcludeId", conn))
            {
                cmd.Parameters.AddWithValue("@Isbn", isbn);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeBookId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public bool HasActiveBorrowings(int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Borrowings WHERE BookId=@Id AND Status IN ('Active','Overdue')", conn))
            {
                cmd.Parameters.AddWithValue("@Id", bookId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public int Add(Book book)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO Books (ISBN, Title, Author, Publisher, CategoryId, PublicationYear,
                                    Quantity, AvailableQuantity, ShelfLocation, Status, CreatedAt)
                OUTPUT INSERTED.BookId
                VALUES (@ISBN, @Title, @Author, @Publisher, @CategoryId, @PublicationYear,
                        @Quantity, @AvailableQuantity, @ShelfLocation, @Status, GETDATE())", conn))
            {
                AddCommonParams(cmd, book);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Update(Book book)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE Books SET ISBN=@ISBN, Title=@Title, Author=@Author, Publisher=@Publisher,
                       CategoryId=@CategoryId, PublicationYear=@PublicationYear, Quantity=@Quantity,
                       AvailableQuantity=@AvailableQuantity, ShelfLocation=@ShelfLocation, Status=@Status
                WHERE BookId=@BookId", conn))
            {
                AddCommonParams(cmd, book);
                cmd.Parameters.AddWithValue("@BookId", book.BookId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int bookId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Books WHERE BookId=@Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", bookId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Atomically increases/decreases AvailableQuantity (delta can be +1 or -1).
        /// The WHERE clause guards against ever going negative, which doubles as a
        /// safety net even if the calling Service layer already checked availability.
        /// </summary>
        public void AdjustAvailableQuantity(int bookId, int delta)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE Books SET AvailableQuantity = AvailableQuantity + @Delta
                WHERE BookId = @Id AND AvailableQuantity + @Delta >= 0", conn))
            {
                cmd.Parameters.AddWithValue("@Delta", delta);
                cmd.Parameters.AddWithValue("@Id", bookId);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    throw new InvalidOperationException("This book has no available copies to issue.");
            }
        }

        private static void AddCommonParams(SqlCommand cmd, Book book)
        {
            cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@Publisher", (object)book.Publisher ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CategoryId", book.CategoryId);
            cmd.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
            cmd.Parameters.AddWithValue("@Quantity", book.Quantity);
            cmd.Parameters.AddWithValue("@AvailableQuantity", book.AvailableQuantity);
            cmd.Parameters.AddWithValue("@ShelfLocation", (object)book.ShelfLocation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", book.Status ?? "Active");
        }

        private static Book Map(IDataRecord r) => new Book
        {
            BookId = (int)r["BookId"],
            ISBN = (string)r["ISBN"],
            Title = (string)r["Title"],
            Author = (string)r["Author"],
            Publisher = r["Publisher"] as string,
            CategoryId = (int)r["CategoryId"],
            CategoryName = (string)r["CategoryName"],
            PublicationYear = (int)r["PublicationYear"],
            Quantity = (int)r["Quantity"],
            AvailableQuantity = (int)r["AvailableQuantity"],
            ShelfLocation = r["ShelfLocation"] as string,
            Status = (string)r["Status"],
            CreatedAt = (DateTime)r["CreatedAt"]
        };
    }
}
