using System;
using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class BorrowingService
    {
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IFineRepository _fineRepository;

        public BorrowingService(
            IBorrowingRepository borrowingRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            IFineRepository fineRepository)
        {
            _borrowingRepository = borrowingRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _fineRepository = fineRepository;
        }

        public List<Borrowing> GetActiveBorrowings() => _borrowingRepository.GetActive();

        public List<Borrowing> GetActiveBorrowingsForMember(int memberId) => _borrowingRepository.GetActiveByMember(memberId);

        public List<Borrowing> FindBorrowingHistory(BorrowingFilter filter) => _borrowingRepository.Find(filter ?? new BorrowingFilter());

        public int IssueBook(int bookId, int memberId, int issuedByUserId, DateTime issueDate, DateTime dueDate)
        {
            var book = _bookRepository.GetById(bookId);
            if (book == null)
                throw new ServiceException("Selected book was not found.");
            if (book.AvailableQuantity <= 0)
                throw new ServiceException($"'{book.Title}' has no available copies right now.");

            var member = _memberRepository.GetById(memberId);
            if (member == null)
                throw new ServiceException("Selected member was not found.");

            if (dueDate < issueDate)
                throw new ServiceException("Due date cannot be earlier than the issue date.");

            var borrowing = new Borrowing
            {
                BookId = bookId,
                MemberId = memberId,
                IssuedBy = issuedByUserId,
                IssueDate = issueDate,
                DueDate = dueDate
            };

            int borrowingId = _borrowingRepository.Add(borrowing);

            _bookRepository.AdjustAvailableQuantity(bookId, -1);

            return borrowingId;
        }

        public decimal ReturnBook(int borrowingId, DateTime returnDate)
        {
            var borrowing = _borrowingRepository.GetById(borrowingId);
            if (borrowing == null)
                throw new ServiceException("Borrowing record not found.");
            if (borrowing.Status == "Returned")
                throw new ServiceException("This book has already been returned.");

            _borrowingRepository.MarkReturned(borrowingId, returnDate);
            _bookRepository.AdjustAvailableQuantity(borrowing.BookId, +1);

            decimal fineAmount = 0m;
            int overdueDays = (returnDate.Date - borrowing.DueDate.Date).Days;
            if (overdueDays > 0)
            {
                fineAmount = overdueDays * AppConstants.FinePerOverdueDay;
                _fineRepository.Add(new Fine
                {
                    BorrowingId = borrowingId,
                    Amount = fineAmount,
                    Reason = $"Returned {overdueDays} day(s) late"
                });
            }

            return fineAmount;
        }

        public int RefreshOverdueStatuses()
        {
            var overdue = _borrowingRepository.GetOverdueActive();
            foreach (var b in overdue)
            {
                if (b.Status != "Overdue")
                    _borrowingRepository.UpdateStatus(b.BorrowingId, "Overdue");
            }
            return overdue.Count;
        }
    }
}
