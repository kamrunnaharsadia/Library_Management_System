using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public interface IRoleRepository
    {
        List<Role> GetAll();
        int GetRoleIdByName(string roleName);
    }
}
