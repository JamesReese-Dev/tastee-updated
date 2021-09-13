using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using E2.Tastee.Common;
using E2.Tastee.Common.ViewModels;
using E2.Tastee.Contracts.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using E2.Tastee.Contracts.Services.Dtos;
using E2.Tastee.Web.ActionFilters;
using E2.Tastee.Web.Extensions;

namespace E2.Tastee.Web.Controllers
{
    [Route("api/[controller]")]
    public class PublicController : BaseController
    {
        public PublicController(IUserService userService, AppSettings appSettings, IMapper mapper, IReferenceService referenceService) :
            base(appSettings, mapper, userService, referenceService)
        {
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationAttemptVM userAuth)
        {
            //Dev
            //if (userAuth.Username == null && userAuth.Password == null && _appSettings.DeveloperModeUserId.HasValue)
            //{
            //    var user = await _userService.GetUserById(_appSettings.DeveloperModeUserId.Value);
            //    user.Token = generateToken(user);
            //    return Ok(new AuthenticationResultDto()
            //    {
            //        User = user
            //    });
            //}

            var result = await _userService.AttemptAuthenticationAsync(userAuth.Username, userAuth.Password, this.Request.HttpContext.Connection.RemoteIpAddress);
            if (!result.Success)
            {
                return FailureResult(result.Message);
            }
            result.User.Token = generateToken(result.User);
            return Ok(new AuthenticationResultDto() { 
                User = result.User
            });
        }

        [Authorize(Policy = AppConstants.POLICIES.TenantOrSystemAdmin)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Impersonate([FromBody] AuthenticationAttemptVM userAuth)
        {
            var result = await _userService.ImpersonationAuthenticationAsync(userAuth.Username, this.Request.HttpContext.Connection.RemoteIpAddress);
            if (!result.Success)
            {
                return FailureResult(result.Message);
            }
            result.User.Token = generateToken(result.User);
            return Ok(new AuthenticationResultDto()
            {
                User = result.User
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] AuthenticationAttemptVM userAuth)
        {
            try
            {
                await _userService.GenerateAndSendForgotPasswordEmail(userAuth.Username);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SetPassword([FromBody] AuthenticationAttemptVM userAuth)
        {
            try
            {
                Guid guidToken; 
                if (Guid.TryParse(userAuth.Token, out guidToken))
                {
                    await _userService.SetPasswordByToken(guidToken, userAuth.Password);
                    return SuccessResult();
                }
                return FailureResult("Invalid password token");
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public virtual async Task<ActionResult> UpdatePassword([FromBody] AuthenticationAttemptVM userAuth)
        {
            var currentUser = await GetCurrentUser();
            try
            {
                _userService.SetPassword(currentUser.Id.Value, userAuth.OldPassword, userAuth.Password);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [Authorize(Policy = AppConstants.POLICIES.AnyUser)]
        [HttpPost("[action]")]
        public virtual async Task<ActionResult> SaveProfile([FromBody] UserDto dto)
        {
            var currentUser = await GetCurrentUser();
            try
            {
                if (currentUser.Id != dto.Id)
                {
                    throw new ApplicationException(AppConstants.ErrorMessages.AccessDenied);
                }
                await _userService.UpdateProfileAsync(dto);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        [Authorize(Policy = AppConstants.POLICIES.AnyUser)]
        [HttpGet("[action]")]
        public async Task<IActionResult> AuthCheck()
        {
            const string FAILED_MESSAGE = "Failed to authenticate user context";
            try
            {
                var currentUser = await GetCurrentUser();
                if (currentUser == null)
                {
                    throw new ApplicationException(FAILED_MESSAGE);
                }
                return SuccessResult();
            }
            catch
            {
                return FailureResult(FAILED_MESSAGE);
            }
        }

        [Authorize(Policy = AppConstants.POLICIES.AnyUser)]
        [HttpGet("User")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var currentUser = await GetCurrentUser();
                return Json(currentUser);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch current user");
                throw;
            }
        }
    }
}
