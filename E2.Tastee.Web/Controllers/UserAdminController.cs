using System;
using System.Linq;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Threading.Tasks;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.Common;
using E2.Tastee.Common.ViewModels;
using E2.Tastee.Web.ViewModels;

namespace E2.Tastee.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = AppConstants.POLICIES.AnyAdmin)]
    [ApiController]
    public class UserAdminController : BaseController
    {
        public UserAdminController(IUserService userService, AppSettings appSettings, IMapper mapper, IReferenceService referenceService): 
            base(appSettings, mapper, userService, referenceService)
        {
        }

        [HttpPost("List")]
        public async Task<PaginatedListVM<UserDto>> List([FromBody] UserSearchCriteria criteria)
        {
            try
            {
                //var currentUser = await GetCurrentUser();
                //if (isOnlyCompanyAdmin(currentUser))
                //{
                //    criteria.CompanyId = currentUser.CompanyId;
                //}
                var currentUser = await GetCurrentUser();
                criteria.MaxResults = _appSettings.DefaultPageSizeInRows;
                var paginatedList = await _userService.FindPaginatedUsersAsync(criteria, currentUser);
                return ToPaginatedListVM(paginatedList, _appSettings.DefaultPageSizeInRows);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch user list");
                throw;
            }
        }

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try { 
                var currentUser = await GetCurrentUser();
                var u = await _userService.GetUserAsync(userId, currentUser);
                return Json(u);
            } catch (Exception ex){
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("[action]/{id}")]
        public virtual async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var currentUser = await GetCurrentUser();
                await _userService.Delete(id, currentUser);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("[action]/{id}")]
        public virtual async Task<ActionResult> ToggleUserActive(int id)
        {
            var currentUser = await GetCurrentUser();
            try
            {
                var u = await _userService.GetUserAsync(id, currentUser);
                await _userService.ToggleUserActive(id, currentUser);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("export")]
        public virtual async Task<IActionResult> GetUsersXLSX([FromBody] UserSearchCriteria criteria)
        {
            var currentUser = await GetCurrentUser();
            var (fileName, fileContents) = await _userService.CreateUsersXLSX(criteria, currentUser);
            return File(fileContents, AppConstants.MimeTypes.XLSX, fileName);
        }

        [HttpPost("[action]/{userId}")]
        public virtual async Task<ActionResult> ToggleMustChangePassword(int userId)
        {
            var currentUser = await GetCurrentUser();
            try
            {
                await _userService.ToggleMustChangePassword(userId, currentUser);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("[action]/{userId}")]
        public virtual async Task<ActionResult> UnlockUser(int userId)
        {
            var currentUser = await GetCurrentUser();
            try
            {
                await _userService.UnlockUserAsync(userId, currentUser);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public virtual async Task<ActionResult> SaveUser([FromBody] UserDto dto)
        {
            var currentUser = await GetCurrentUser();
            try
            {
                if (dto.Id > 0)
                {
                    await _userService.UpdateUserAsync(dto, currentUser);
                }
                else
                {
                    dto = await _userService.AddUserAsync(dto, currentUser);
                }
                return SuccessResult(dto);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.Message.ToLower().Contains("cannot insert duplicate key"))
                {
                    return FailureResult(
                        String.Format(
                            "{0} {1} [{2}] has already been defined and cannot be added a second time.\nPlease find and update the existing user if changes need to be made.",
                                dto.FirstName, dto.LastName, dto.Username)
                    );
                }
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Impersonate([FromBody] AuthenticationAttemptVM vm) 
        {
            try
            {
                var currentUser = await GetCurrentUser();
                var criteria = new UserSearchCriteria() { ActiveOnly = true, Username = vm.Username };
                var userList = await _userService.FindUsersAsync(criteria, currentUser);
                var user = userList.FirstOrDefault();
                if (user == null)
                {
                    return FailureResult($"No active user with the username {vm.Username} could be found.");
                }
                user.Token = generateToken(user);
                return Ok(new AuthenticationResultDto()
                {
                    User = user
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error trying to impersonate");
                return FailureResult(ex.Message);
            }
        }
    }
}
