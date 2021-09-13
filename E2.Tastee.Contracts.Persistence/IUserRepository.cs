using E2.Tastee.Common;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E2.Tastee.Contracts.Persistence
{
    public interface IUserRepository : IRepository
    {
        Task<List<User>> FindUsersAsync(UserSearchCriteria criteria, UserDto currentUser);
        Task<PaginatedList<User>> FindPaginatedUsersAsync(UserSearchCriteria criteria, UserDto currentUser);
        Task HardDeleteUser(int userId);
        Task<string> GetDefaultSubdomain(int userId);
    }
}
