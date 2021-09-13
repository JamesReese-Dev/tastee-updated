using E2.Tastee.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Contracts.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticationResultDto> ImpersonationAuthenticationAsync(string username, System.Net.IPAddress clientIP = null);
        Task<AuthenticationResultDto> AttemptAuthenticationAsync(string username, string password, System.Net.IPAddress clientIP = null);
        Task<UserDto> GetUserAsync(int userId, UserDto currentUser);
        Task<Tuple<string, byte[]>> CreateUsersXLSX(UserSearchCriteria criteria, UserDto currentUser);
        Task<IList<UserDto>> FindUsersAsync(UserSearchCriteria criteria, UserDto currentUser);
        Task<PaginatedList<UserDto>> FindPaginatedUsersAsync(UserSearchCriteria criteria, UserDto currentUser);
        Task<UserDto> AddUserAsync(UserDto dto, UserDto currentUser);
        Task GenerateAndSendForgotPasswordEmail(string username);
        void SetPassword(int userId, string oldPassword, string newPassword);
        Task SetPasswordByToken(Guid token, string newPassword);
        Task UpdateProfileAsync(UserDto dto);
        Task UpdateUserAsync(UserDto dto, UserDto currentUser);
        Task ToggleUserActive(int userId, UserDto currentUser);
        Task Delete(int userId, UserDto currentUser);
        Task<bool> UserExistsAsync(string username, int? existingUserId = null);
        Task UnlockUserAsync(int userId, UserDto currentUser);
        Task ToggleMustChangePassword(int userId, UserDto currentUser, bool sendResetEmail = true);
        Task UpdatePasswordAsync(int userId, string password);
    }
}