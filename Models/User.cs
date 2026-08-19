using System;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }   
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }   
        public string Status { get; set; }    
        public DateTime CreatedAt { get; set; }

        public bool IsActive => Status == "Active";
    }
}
