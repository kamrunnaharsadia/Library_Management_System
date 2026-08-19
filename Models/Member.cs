using System;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public int UserId { get; set; }
        public string StudentId { get; set; }
        public string Department { get; set; }
        public string Semester { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Convenience fields populated by JOIN with Users, useful for grids
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }
}
