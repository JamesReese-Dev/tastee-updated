using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E2.Tastee.Common;
using E2.Tastee.Common.Dtos;
using E2.Tastee.Contracts.Services.Dtos;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.Models;
using E2.Tastee.Web.ActionFilters;
using E2.Tastee.Web.ViewModels;

namespace E2.Tastee.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IUserService userService, AppSettings appSettings,
            IMapper mapper, IAdminService adminService, IReferenceService referenceService) 
            : base(appSettings, mapper, userService, referenceService)
        {
            _adminService = adminService;
        }

        private async Task<IActionResult> toggleItemActive(IdVM vm, string methodName, Func<int, UserDto, Task> toggle)
        {
            try
            {
                var currentUser = await GetCurrentUser();
                await toggle(vm.Id, currentUser);
                return SuccessResult();
            }
            catch (ApplicationException aex)
            {
                return FailureResult(aex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in AdminController/" + methodName);
                return FailureResult(ex.Message);
            }
        }

        private async Task<IActionResult> saveItem<T>(T dto, string methodName, Func<T, UserDto, Task<T>> save)
        {
            try
            {
                var currentUser = await GetCurrentUser();
                await save(dto, currentUser);
                return SuccessResult();
            }
            catch (ApplicationException aex)
            {
                return FailureResult(aex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in AdminController/" + methodName);
                return FailureResult(ex.Message);
            }
        }

    }
}
