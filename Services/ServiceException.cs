using System;

namespace LibraryManagementSystem.Services
{
    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
    }
}
