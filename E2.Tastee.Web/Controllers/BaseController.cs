using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using HeyRed.Mime;
using Serilog;
using E2.Tastee.Contracts.Services.Dtos;
using Newtonsoft.Json;

namespace E2.Tastee.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUserService _userService;
        protected readonly IReferenceService _referenceService;
        protected readonly IMapper _mapper;
        protected readonly AppSettings _appSettings;

        public BaseController(AppSettings appSettings, IMapper mapper, IUserService userService, IReferenceService referenceService)
        {
            _mapper = mapper;
            _appSettings = appSettings;
            _userService = userService;
            _referenceService = referenceService;
        }

        private string getBaseUrl() => $"{this.Request.Scheme}://{this.Request.Host}";

        private string getSubdomain()
        {
            var subDomain = string.Empty;
            var host = this.Request.Host.Host;
            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }
            if (subDomain.EndsWith("-qa"))
            {
                subDomain = subDomain.Replace("-qa", String.Empty);
            }
            return subDomain.Trim().ToLower();
        }

        protected async Task<PaginatedListVM<SoftDeletableSimpleDto>> getPaginatedSimpleList<T>(SimpleCriteria criteria) where T : ISoftDeletableDto, ISimpleDto
        {
            try
            {
                var paginatedList = await _referenceService.GetPaginatedListAsync<T>(criteria);
                return ToPaginatedListVM(paginatedList, _appSettings.DefaultPageSizeInRows);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to fetch {typeof(T).FullName} list");
                throw;
            }
        }

        protected async Task<ActionResult> toggleActive<T>(int id) where T : ISoftDeletableDto
        {
            var currentUser = await GetCurrentUser();
            try
            {
                await _referenceService.ToggleActiveAsync<T>(id, currentUser);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        protected async Task<ActionResult> saveSimpleItem<T>([FromBody] SoftDeletableSimpleDto dto) where T : ISoftDeletableDto, ITrackedSimpleDto
        {
            var currentUser = await GetCurrentUser();
            try
            {
                dto = await _referenceService.SaveAsync<T>(new TrackedSimpleDto()
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    CreatedByUserId = currentUser.Id.Value,
                    DeactivatedAt = dto.DeactivatedAt
                }, currentUser);
                return SuccessResult();
            }
            catch (Exception ex)
            {
                return FailureResult(ex.Message);
            }
        }

        protected string generateToken(UserDto user)
        {
            var claims = new List<Claim> {
                new Claim(AppConstants.CLAIM_TYPE_USER_ID, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            foreach (UserRoleDto ur in user.Roles)
            {
                claims.Add(
                    new Claim(ClaimTypes.Role, ((int)ur.TypeOfUserRole).ToString()));
            }
            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtValidIssuer,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSecret)),
                    SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected async Task<byte[]> blobContentToBytes(IFormFile content)
        {
            using (var memoryStream = new MemoryStream())
            {
                await content.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        protected async Task<UserDto> GetCurrentUser()
        {
            try
            {
                if (this.User == null)
                {
                    Log.Warning($"currentUser controller.user is null");
                    return null;
                }
                var userId = this.User.FindFirstValue(AppConstants.CLAIM_TYPE_USER_ID);
                if (!string.IsNullOrEmpty(userId) && this.User.Identity != null && this.User.Identity.IsAuthenticated)
                    return await _userService.GetUserAsync(int.Parse(userId), AppConstants.ResourceUser());

                var username = this.User.FindFirstValue(ClaimTypes.Name);
                Log.Warning($"GetCurrentUser user id claims is null and user is not authenticated, attempting user lookup by username = [{username}], all claims = [{JsonConvert.SerializeObject(this.User.Claims.ToList())}]");
                if (String.IsNullOrEmpty(username))
                {
                    return null;
                }
                var users = await _userService.FindUsersAsync(new UserSearchCriteria()
                {
                    ActiveOnly = true,
                    Username = username
                }, AppConstants.ResourceUser());
                return users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during currentUser");
                return null;
            }
        }

        protected PaginatedListVM<T> ToPaginatedListVM<T>(PaginatedList<T> paginatedList, int pageSize)
        {
            return new PaginatedListVM<T>()
            {
                List = new CustomPagination<T>(paginatedList.Items, paginatedList.PageNumber, pageSize, paginatedList.TotalCount),
                PageNumber = paginatedList.PageNumber,
                TotalPages = (paginatedList.TotalCount / pageSize) + (paginatedList.TotalCount % pageSize == 0 ? 0 : 1)
            };
        }

        public static JsonResult SuccessResult(object message = null)
        {
            return new JsonResult(new { Success = true, Message = message ?? String.Empty })
            {
                ContentType = AppConstants.MimeTypes.JSON
                // JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public static JsonResult FailureResult(string message)
        {
            return new JsonResult(new { Success = false, Message = message })
            {
                ContentType = AppConstants.MimeTypes.JSON
                // JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        protected FileContentResult ExportExcel(byte[] excelBytes, string fileNameWithoutExtension)
        {
            return File(excelBytes, MimeTypesMap.GetMimeType(".xlsx"), $"{fileNameWithoutExtension}.xlsx");
        }
        protected FileContentResult ExportCsv(byte[] csvBytes, string fileNameWithoutExtension)
        {
            return File(csvBytes, MimeTypesMap.GetMimeType(".csv"), $"{fileNameWithoutExtension}.csv");
        }
        protected FileContentResult ExportPdf(byte[] pdfBytes, string fileNameWithoutExtension)
        {
            return File(pdfBytes, MimeTypesMap.GetMimeType(".pdf"), $"{fileNameWithoutExtension}.pdf");
        }
    }
}
