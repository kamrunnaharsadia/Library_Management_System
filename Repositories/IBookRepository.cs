using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class BookFilter
    {
        public string Keyword { get; set; }     
        public int? CategoryId { get; set; }
        public bool? AvailableOnly { get; set; }
        public string Status { get; set; }
    }

    public interface IBookRepository
    {
        List<Book> GetAll();
        Book GetById(int bookId);
        List<Book> Find(BookFilter filter);
        bool IsbnExists(string isbn, int excludeBookId = 0);
        bool HasActiveBorrowings(int bookId);
        int Add(Book book);
        void Update(Book book);
        void Delete(int bookId);
        void AdjustAvailableQuantity(int bookId, int delta);
    }
}
