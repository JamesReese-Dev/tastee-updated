using System;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Threading.Tasks;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.Common;
using System.Collections.Generic;
using E2.Tastee.Models;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = AppConstants.POLICIES.AnyUser)]
    [ApiController]
    public class ReferenceController : BaseController
    {
        private readonly IAdminService _adminService;

        public ReferenceController(IUserService userService, AppSettings appSettings,
            IAdminService adminService, IMapper mapper, IReferenceService referenceService) : 
            base(appSettings, mapper, userService, referenceService)
        {
            _adminService = adminService;
        }

        [HttpGet("[action]")]
        public List<string> TimezoneList()
            => AppHelpers.GetTimezoneList();

        [HttpGet("[action]")]
        public async Task<List<SimpleDto>> Users()
        {
            var currentUser = await GetCurrentUser();
            var users = await _userService.FindUsersAsync(new UserSearchCriteria() { ActiveOnly = true, ExcludeSystemUser = false }, currentUser);
            return _mapper.Map<IList<UserDto>, List<SimpleDto>>(users);
        }
    }
}
