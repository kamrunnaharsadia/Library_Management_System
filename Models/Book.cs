using System;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }  // populated by JOIN
        public int PublicationYear { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string ShelfLocation { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsAvailable => AvailableQuantity > 0 && Status == "Active";
    }
}
