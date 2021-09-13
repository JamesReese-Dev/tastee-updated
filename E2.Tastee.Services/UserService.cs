using AutoMapper;
using Serilog;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeOpenXml;
using E2.Tastee.Common.Extensions;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Services
{
    public class UserService : BaseDataService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICipherService _cipherService;

        public UserService(IReferenceRepository referenceRepository, IMapper mapper, IUserRepository userRepository,
            IUnitOfWork unitOfWork, ICipherService cipherService,
            AppSettings appSettings)
            : base(referenceRepository, mapper, appSettings, unitOfWork)
        {
            _userRepository = userRepository;
            _cipherService = cipherService;
        }

        private void pushUpdatableUserProfileToModel(User model, UserDto dto, int currentUserId)
        {
            model.FirstName = dto.FirstName;
            model.LastName = dto.LastName;
            model.Email = dto.Email;
            model.Timezone = dto.Timezone;
            model.MobilePhoneNumber = dto.MobilePhoneNumber ?? String.Empty;
            model.LastUpdatedByUserId = currentUserId;
            model.UpdatedAt = DateTime.UtcNow;
            model.MothersMaidenName = dto.MothersMaidenName;
            model.UserBloodType = dto.UserBloodType;
        }

        private void pushUpdatableUserFieldsToModel(User model, UserDto dto, int currentUserId)
        {
            pushUpdatableUserProfileToModel(model, dto, currentUserId);
            model.Username = dto.Username;
            model.MustChangePassword = dto.MustChangePassword ?? false;
        }

        private static void validatePassword(string v)
        {
            if (String.IsNullOrWhiteSpace(v))
            {
                throw new ApplicationException("A password value is required.");
            }
            if (!Regex.IsMatch(v, AppConstants.PASSWORD_COMPLEXITY_REGEX))
            {
                throw new ApplicationException("The password provided does not meet the minimum complexity requirements (" +
                    AppConstants.PASSWORD_COMPLEXITY_DSC + ")");
            }
        }

        private void validateUserModel(User user, IList<UserRoleDto> proposedRoleList)
        {
            if (String.IsNullOrWhiteSpace(user.Timezone))
                user.Timezone = AppConstants.DEFAULT_TIME_ZONE;
            HashSet<string> validationErrors = new HashSet<string>();
            if (String.IsNullOrWhiteSpace(user.Username))
                validationErrors.Add("Username is required");
            if (String.IsNullOrWhiteSpace(user.FirstName)
                || String.IsNullOrWhiteSpace(user.LastName))
                validationErrors.Add("Both first and last name are required");
            if (String.IsNullOrWhiteSpace(user.HashedPassword)
                || String.IsNullOrWhiteSpace(user.PasswordSalt))
                validationErrors.Add("The proposed password value is not valid");
            if (proposedRoleList == null || proposedRoleList.Count == 0)
                validationErrors.Add("The provided user has an invalid associated list of roles");
            if (validationErrors.Any())
                throw new ApplicationException(String.Join(",", validationErrors));
        }

        public async Task<UserDto> AddUserAsync(UserDto dto, UserDto currentUser)
        {
            try
            {
                _unitOfWork.Begin();
                var user = new User();
                pushUpdatableUserFieldsToModel(user, dto, currentUser.Id.Value);
                validatePassword(dto.Password);
                // these properties are only set once upon user creation
                user.PasswordSalt = _cipherService.GenerateSalt();
                user.HashedPassword = _cipherService.ComputeSHA256Hash(dto.Password, user.PasswordSalt);
                user.FailedAttemptCount = 0;
                user.CreatedByUserId = currentUser.Id.Value;
                validateUserModel(user, dto.Roles);
                var exists = await UserExistsAsync(dto.Username, null);
                if (exists)
                {
                    throw new ApplicationException($"The username {dto.Username} has already been taken.  Please either register with another username or login if you have already registered.");
                }
                user = _userRepository.Create(user);
                setUserRoles(user, dto.Roles, currentUser);
                var newUserDto = _mapper.Map<User, UserDto>(user);
                _unitOfWork.Commit();
                return newUserDto;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user addition");
                throw;
            }
        }

        public async Task<AuthenticationResultDto> ImpersonationAuthenticationAsync(string Username, System.Net.IPAddress clientIP = null)
        {
            AuthenticationResultDto result = new AuthenticationResultDto();
            var user = await getUserByUsernameAsync(Username);
            if (user == null)
            {
                result.Message = "User could not be found";
            }
            else
            {
                result.User = user;
            }
            _referenceRepository.Create(new UserAuthenticationAttempt()
            {
                OccurredAt = DateTime.UtcNow,
                Username = Username,
                WasSuccessful = result.Success,
                IP = clientIP == null ? null : clientIP.GetAddressBytes()
            });
            return result;
        }

        public async Task<AuthenticationResultDto> AttemptAuthenticationAsync(string Username, string Password, IPAddress clientIP)
        {
            AuthenticationResultDto result = new AuthenticationResultDto();
            bool success = false;
            try
            {
                _unitOfWork.Begin();
                var list = await _userRepository.FindUsersAsync(new UserSearchCriteria()
                {
                    ActiveOnly = true,
                    Username = Username,
                    ExcludeSystemUser = false
                }, AppConstants.ResourceUser());
                var user = list.FirstOrDefault();
                if (user == null)
                {
                    result.Message = "This user account is unavailable.";
                }
                else if (user.DeactivatedAt == null)
                {
                    if (user.LastLockedOutAt.HasValue)
                    {
                        if (DateTime.UtcNow.Subtract(user.LastLockedOutAt.Value).TotalMinutes < AppConstants.LOGON_SELF_HEAL_MINUTES)
                        {
                            result.Message = String.Format("This account is currently locked out. You can wait {0} minutes before trying again or use the password reset features.",
                                AppConstants.LOGON_SELF_HEAL_MINUTES);
                        }
                        else // unlock them and give them a fresh set of tries
                        {
                            user.LastLockedOutAt = null;
                            user.FailedAttemptCount = 0;
                            await _userRepository.UpdateAsync(user);
                        }
                    }
                    if (user.LastLockedOutAt == null)
                    {
                        success = _cipherService.SHA256HashMatches(Password, user.PasswordSalt, user.HashedPassword);
                        if (!success)
                        {
                            result.Message = "Invalid password";
                            user.FailedAttemptCount++;
                            if (user.FailedAttemptCount == AppConstants.MAX_FAILED_LOGON_ATTEMPTS)
                            {
                                result.Message = String.Format("You've entered bad credentials {0} times. Be careful. You'll be locked out after your next failed attempt.",
                                    AppConstants.MAX_FAILED_LOGON_ATTEMPTS);
                            }
                            if (user.FailedAttemptCount > AppConstants.MAX_FAILED_LOGON_ATTEMPTS)
                            {
                                user.LastLockedOutAt = DateTime.UtcNow;
                                result.Message = String.Format("You've failed to logon more than {0} times. Either use the forgot password feature or wait {1} minutes before trying again.",
                                    AppConstants.MAX_FAILED_LOGON_ATTEMPTS, AppConstants.LOGON_SELF_HEAL_MINUTES);
                            }
                            await _userRepository.UpdateAsync(user);
                        }
                    }
                }
                else
                {
                    result.Message = "This user account is inactive. Contact an administrator.";
                }
                if (success)
                {
                    user.LastLoggedOnAt = DateTime.UtcNow;
                    user.LastLockedOutAt = null;
                    await _userRepository.UpdateAsync(user);
                }
                _referenceRepository.Create(new UserAuthenticationAttempt()
                {
                    OccurredAt = DateTime.UtcNow,
                    Username = Username,
                    WasSuccessful = success,
                    FailureReason = result.Message,
                    IP = clientIP == null ? null : clientIP.GetAddressBytes()
                });
                result.User = _mapper.Map<User, UserDto>(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user attempt authentication");
                result.Message = "Error during authentication";
                //throw;
            }
            return result;
        }

        public async Task<IList<UserDto>> FindUsersAsync(UserSearchCriteria criteria, UserDto currentUser)
        {
            return _mapper.Map<IList<User>, IList<UserDto>>(
                await _userRepository.FindUsersAsync(criteria, currentUser));
        }

        public async Task<PaginatedList<UserDto>> FindPaginatedUsersAsync(UserSearchCriteria criteria, UserDto currentUser)
        {
            return _mapper.Map<PaginatedList<User>, PaginatedList<UserDto>>(
                await _userRepository.FindPaginatedUsersAsync(criteria, currentUser));
        }

        private async Task ensureAccessToUser(UserDto currentUser, User user)
        {
            if (currentUser != null
                && user != null
                && AppHelpers.RequiresUserAccessConstraints(currentUser)
                && user.Id != currentUser.Id)
            {
                var userAccessList = await _userRepository.FindUsersAsync(new UserSearchCriteria()
                {
                    Id = user.Id
                }, currentUser);
                if (!userAccessList.Any())
                {
                    throw new ApplicationException(AppConstants.ErrorMessages.AccessDenied);
                }
            }
        }

        public async Task<Tuple<string, byte[]>> CreateUsersXLSX(UserSearchCriteria criteria, UserDto currentUser)
        {
            var excelPackage = new ExcelPackage();
            var ws = excelPackage.Workbook.Worksheets.Add("Participants");

            string fileName = $"users_{DateTime.UtcNow.FromUtcToTimezone(currentUser.Timezone).ToString("dMMMyyyy")}.xlsx";

            //headers
            ws.Cells[1, 1].Value = "First Name";
            ws.Cells[1, 2].Value = "Last Name";
            ws.Cells[1, 3].Value = "Email";
            ws.Cells[1, 4].Value = "Username";
            ws.Cells[1, 5].Value = "Mobile Phone Number";
            ws.Cells[1, 6].Value = "Roles";


            var row = 2;
            var users = await FindUsersAsync(criteria, currentUser);
            foreach (var u in users)
            {
                string roleString = u.Roles != null && u.Roles.Any() 
                    ? u.Roles.Select(x => $"({((x.CGName ?? x.MGName ?? x.CategoryName ?? x.SubcategoryName ?? String.Empty) + " ").TrimEnd() + x.RoleName})").Aggregate((x, y) => x + ", " + y) 
                    : String.Empty;
                ws.Cells[row, 1].Value = u.FirstName;
                ws.Cells[row, 2].Value = u.LastName;
                ws.Cells[row, 3].Value = u.Email;
                ws.Cells[row, 4].Value = u.Username;
                ws.Cells[row, 5].Value = u.MobilePhoneNumber;
                ws.Cells[row, 6].Value = roleString;
                row++;
            }
            return new Tuple<string, byte[]>(fileName, excelPackage.GetAsByteArray());
        }

        public async Task<UserDto> GetUserAsync(int userId, UserDto currentUser)
        {
            var users = await _userRepository.FindUsersAsync(new UserSearchCriteria() { ActiveOnly = true, Id = userId }, currentUser);
            var user = users.FirstOrDefault();
            await ensureAccessToUser(currentUser, user);
            return _mapper.Map<User, UserDto>(user);
        }

        private async Task<UserDto> getUserByUsernameAsync(string username, int? existingUserId = null)
        {
            var list = await _userRepository.FindUsersAsync(new UserSearchCriteria()
            {
                ActiveOnly = true,
                Username = username,
                ExceptUserId = existingUserId
            }, null);
            return _mapper.Map<User, UserDto>(list.FirstOrDefault());
        }

        public async Task ToggleMustChangePassword(int userId, UserDto currentUser, bool sendResetEmail = true)
        {
            try
            {
                _unitOfWork.Begin();
                User user = await _userRepository.GetAsync<User>(userId);
                if (user != null)
                {
                    await ensureAccessToUser(currentUser, user);
                    user.MustChangePassword = !user.MustChangePassword;
                    if (user.MustChangePassword && sendResetEmail)
                    {
                        await GenerateAndSendForgotPasswordEmail(user.Username);
                    }
                    await _userRepository.UpdateAsync(user);
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user toggle must change password");
                throw;
            }
        }

        private void setUserPasswordFields(User user, string newPassword)
        {
            validatePassword(newPassword);
            user.HashedPassword = _cipherService.ComputeSHA256Hash(newPassword, user.PasswordSalt);
            user.ResetPasswordToken = null;
            user.FailedAttemptCount = 0;
            user.LastLockedOutAt = null;
            user.MustChangePassword = false;
        }

        public async void SetPassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                _unitOfWork.Begin();
                User user = await _userRepository.GetAsync<User>(userId);
                if (user == null) throw new ApplicationException("The specified user could not be found!");
                if (!_cipherService.SHA256HashMatches(oldPassword, user.PasswordSalt, user.HashedPassword))
                    throw new ApplicationException("The provided, existing password is incorrect");
                setUserPasswordFields(user, newPassword);
                await _userRepository.UpdateAsync(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user SetPassword");
                throw;
            }
        }

        public async Task SetPasswordByToken(Guid token, string newPassword)
        {
            try
            {
                _unitOfWork.Begin();
                var list = await _userRepository.FindUsersAsync(new UserSearchCriteria()
                {
                    ActiveOnly = true,
                    PasswordResetToken = token
                }, AppConstants.ResourceUser());
                User user = list.FirstOrDefault();
                if (user == null)
                {
                    throw new ApplicationException("The specified user could not be found!");
                }
                if (!user.ResetPasswordExpiresAt.HasValue || user.ResetPasswordExpiresAt.Value < DateTime.UtcNow)
                {
                    throw new ApplicationException("The password reset token is no longer valid.  If you still need to reset your password please request another.");
                }
                setUserPasswordFields(user, newPassword);
                await _userRepository.UpdateAsync(user);
                _unitOfWork.Commit();
            }
            catch (ApplicationException)
            {
                _unitOfWork.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user SetPasswordByToken");
                throw;
            }
        }

        public async Task GenerateAndSendForgotPasswordEmail(string username)
        {
            Guid token = Guid.NewGuid();
            bool wasInTransaction = _unitOfWork.IsInTransaction();
            try
            {
                if (!wasInTransaction) _unitOfWork.Begin();
                var userList = await _userRepository.FindUsersAsync(new UserSearchCriteria()
                {
                    ActiveOnly = true,
                    Username = username
                }, AppConstants.ResourceUser());
                User user = userList.FirstOrDefault();
                if (user == null)
                    throw new ApplicationException("The specified user could not be found!");
                user.ResetPasswordToken = token;
                user.ResetPasswordExpiresAt = DateTime.UtcNow.AddMinutes(30);
                await _userRepository.UpdateAsync(user);
                if (!wasInTransaction) _unitOfWork.Commit();
                string link;
                string defaultSubdomain = await _userRepository.GetDefaultSubdomain(user.Id);
#if DEBUG
                link = string.Format("https://localhost:44375/reset_password/{0}", token);
#else
                link = String.IsNullOrEmpty(defaultSubdomain)
                    ? $"{_appSettings.AppRoot}/reset_password/{token}"
                    : $"https://{defaultSubdomain}.{_appSettings.AppRoot}/reset_password/{token}";
#endif
                string emailBody = "<div><p>Please use the below link to reset your password</p><a href=\"" + link + "\">Reset Password</a><p>Or copy and paste this URL in your browser: " + link + "</p></div><br/>{{app-logo}}";
                // _emailService.SendEmail(user.Email, user.FirstName + " " + user.LastName, _appSettings.AppName + " Password Reset", emailBody);
            }
            catch (Exception ex)
            {
                if (!wasInTransaction) _unitOfWork.Rollback();
                Log.Error(ex, "Error during user SetPasswordRequestReminder, token=" + token);
                throw;
            }
        }

        public async Task Delete(int userId, UserDto currentUser)
        {
            User user = await _userRepository.GetAsync<User>(userId);
            if (user != null)
            {
                await _userRepository.HardDeleteUser(userId);
            }
        }

        public async Task ToggleUserActive(int userId, UserDto currentUser)
        {
            try
            {
                _unitOfWork.Begin();
                User user = await _userRepository.GetAsync<User>(userId);
                if (user == null)
                    throw new ApplicationException("The specified user could not be found!");
                await ensureAccessToUser(currentUser, user);
                if (user.DeactivatedAt == null)
                {
                    user.DeactivatedAt = DateTime.UtcNow;
                    bool wouldBeDuplicateUsername = await UserExistsAsync(user.Username, user.Id);
                    if (wouldBeDuplicateUsername)
                    {
                        throw new ApplicationException($"{user.Username} exists and would create a duplicate.");
                    }
                }
                else
                {
                    user.DeactivatedAt = null;
                }
                await _userRepository.UpdateAsync(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user toggle active");
                throw;
            }
        }

        public async Task UnlockUserAsync(int userId, UserDto currentUser)
        {
            try
            {
                _unitOfWork.Begin();
                User user = await _userRepository.GetAsync<User>(userId);
                if (user != null)
                {
                    await ensureAccessToUser(currentUser, user);
                    user.FailedAttemptCount = 0;
                    user.LastLockedOutAt = null;
                    await _userRepository.UpdateAsync(user);
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user unlock");
                throw;
            }
        }

        private void setUserRoles(User user, IList<UserRoleDto> desiredRoles, UserDto requestingUser = null)
        {
            if (user.Roles == null)
                user.Roles = new List<UserRole>();
            if (desiredRoles == null)
                desiredRoles = new List<UserRoleDto>();
            var existingRoles = user.Roles;
            // eliminate the possibility of
            // ...a non-sys-admin user adding that role to anyone
            if (desiredRoles.Any(x => x.TypeOfUserRole == TypeOfUserRole.Administrator)
                && requestingUser != null
                && !requestingUser.Roles.Any(x => x.TypeOfUserRole == TypeOfUserRole.Administrator))
            {
                throw new ApplicationException("Addition of the system administrator role has been requested by an editing user that is not a system admin - Access Denied!");
            }
            // ...a non-sys-admin editing a sys-admin user
            if (existingRoles.Any(x => x.TypeOfUserRole == TypeOfUserRole.Administrator)
                && requestingUser != null
                && !requestingUser.Roles.Any(x => x.TypeOfUserRole == TypeOfUserRole.Administrator))
            {
                throw new ApplicationException("The target user is a system administrator but the editing user is not - Access Denied!");
            }
            // find and add any roles that don't already exist
            var newRoles = desiredRoles.Where(d => !existingRoles.Any(x => x.TypeOfUserRole == d.TypeOfUserRole)).ToArray();
            // find any roles no longer in the desired list
            var oldRoles = existingRoles.Where(e => !desiredRoles.Any(x => x.TypeOfUserRole == e.TypeOfUserRole)).ToArray();
            if (!newRoles.Any() && !oldRoles.Any()) return;
            foreach (var o in oldRoles)
            {
                var oldRoleModel = user.Roles.First(x => x.TypeOfUserRole == o.TypeOfUserRole);
                user.Roles.Remove(oldRoleModel);
                oldRoleModel.DeactivatedAt = DateTime.UtcNow;
                oldRoleModel.DeactivatedByUserId = requestingUser == null
                    ? AppConstants.RESOURCE_USER_ID
                    : requestingUser.Id;
                _userRepository.Update(oldRoleModel);
            }

            foreach (var n in newRoles)
            {
                var newRoleModel = new UserRole()
                {
                    TypeOfUserRole = n.TypeOfUserRole,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = requestingUser == null
                        ? AppConstants.RESOURCE_USER_ID
                        : requestingUser.Id.Value
                };
                _userRepository.Create(newRoleModel);
                user.Roles.Add(newRoleModel);
            }
        }

        private void ensureIdExists(UserDto dto)
        {
            if (!dto.Id.HasValue || dto.Id.Value <= 0)
            {
                throw new ApplicationException("Missing Id value for update operation");
            }
        }

        public async Task UpdateUserAsync(UserDto dto, UserDto currentUser)
        {
            // if (dto.Id == AppConstants.SYSTEM_USER_ID) return;
            ensureIdExists(dto);
            try
            {
                _unitOfWork.Begin();
                User user = await _userRepository.GetAsync<User>(dto.Id.Value);
                if (user != null)
                {
                    await ensureAccessToUser(currentUser, user);
                    bool wouldBeDuplicateUsername = await UserExistsAsync(dto.Username, user.Id);
                    if (wouldBeDuplicateUsername)
                    {
                        throw new ApplicationException($"{dto.Username} exists and would create a duplicate.");
                    }
                    pushUpdatableUserFieldsToModel(user, dto, currentUser.Id.Value);
                    validateUserModel(user, dto.Roles);
                    await _userRepository.UpdateAsync(user);
                    setUserRoles(user, dto.Roles, currentUser);
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user update");
                throw;
            }
        }

        public async Task UpdatePasswordAsync(int userId, string password)
        {
            try
            {
                _unitOfWork.Begin();
                var user = await _userRepository.GetAsync<User>(userId);
                if (user != null || user.DeactivatedAt == null)
                {
                    user.HashedPassword = _cipherService.ComputeSHA256Hash(password, user.PasswordSalt);
                    await _userRepository.UpdateAsync(user);
                    _unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during password update");
                throw;
            }
        }

        private void validateUserProfile(UserDto user)
        {
            HashSet<string> validationErrors = new HashSet<string>();
            //if (String.IsNullOrWhiteSpace(user.City)
            //    || String.IsNullOrWhiteSpace(user.State))
            //    validationErrors.Add("City and state are both required");
            if (String.IsNullOrWhiteSpace(user.FirstName)
                || String.IsNullOrWhiteSpace(user.LastName))
                validationErrors.Add("Both first and last name are required");
            if (validationErrors.Any())
                throw new ApplicationException(String.Join(",", validationErrors));
        }

        public async Task UpdateProfileAsync(UserDto dto)
        {
            ensureIdExists(dto);
            validateUserProfile(dto);
            try
            {
                _unitOfWork.Begin();
                User user = await _userRepository.GetAsync<User>(dto.Id.Value);
                if (user != null)
                {
                    pushUpdatableUserProfileToModel(user, dto, user.Id);
                    await _userRepository.UpdateAsync(user);
                }
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, "Error during user profile update");
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string username, int? existingUserId = null)
        {
            var result = await getUserByUsernameAsync(username, existingUserId);
            return result != null;
        }
    }
}
