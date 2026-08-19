using System;
using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public List<Book> GetAllBooks() => _bookRepository.GetAll();

        public Book GetBook(int bookId) => _bookRepository.GetById(bookId);

        public List<Book> FindBooks(BookFilter filter) => _bookRepository.Find(filter ?? new BookFilter());

        public int AddBook(Book book)
        {
            Validate(book);
            if (_bookRepository.IsbnExists(book.ISBN))
                throw new ServiceException($"A book with ISBN '{book.ISBN}' already exists.");

            // A brand-new book starts fully available.
            book.AvailableQuantity = book.Quantity;
            book.Status = string.IsNullOrWhiteSpace(book.Status) ? "Active" : book.Status;
            return _bookRepository.Add(book);
        }

        public void UpdateBook(Book book)
        {
            Validate(book);
            if (_bookRepository.IsbnExists(book.ISBN, book.BookId))
                throw new ServiceException($"A book with ISBN '{book.ISBN}' already exists.");

            var existing = _bookRepository.GetById(book.BookId);
            if (existing == null)
                throw new ServiceException("Book not found.");

            // Keep AvailableQuantity consistent if the librarian changes total Quantity:
            // shift AvailableQuantity by the same delta, but never let it go negative
            // or exceed the new total.
            int delta = book.Quantity - existing.Quantity;
            int newAvailable = existing.AvailableQuantity + delta;
            if (newAvailable < 0) newAvailable = 0;
            if (newAvailable > book.Quantity) newAvailable = book.Quantity;
            book.AvailableQuantity = newAvailable;

            _bookRepository.Update(book);
        }

        public void DeleteBook(int bookId)
        {
            // Business rule: cannot delete a book that is currently on loan.
            if (_bookRepository.HasActiveBorrowings(bookId))
                throw new ServiceException("This book cannot be deleted because it currently has active borrowings.");
            _bookRepository.Delete(bookId);
        }

        private void Validate(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.ISBN))
                throw new ServiceException("ISBN is required.");
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ServiceException("Title is required.");
            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ServiceException("Author is required.");
            if (book.CategoryId <= 0)
                throw new ServiceException("Please select a category.");
            if (book.PublicationYear < 1400 || book.PublicationYear > DateTime.Now.Year + 1)
                throw new ServiceException("Please enter a valid publication year.");
            if (book.Quantity < 0)
                throw new ServiceException("Quantity cannot be negative.");
        }
    }
}
