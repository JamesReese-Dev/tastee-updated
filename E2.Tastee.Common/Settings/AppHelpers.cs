using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Common
{
    public static class AppHelpers
    {
        public static string ToSignedURL(AppSettings s, string fileKey, string signatureSegment)
            => $@"https://{s.AzureBlobAccount}.{s.AzureBlobURL}/{s.DocumentStorageContainerName}/{fileKey}{signatureSegment}";

        public static bool IsSortAscending(string sortDirection)
        {
            return String.IsNullOrEmpty(sortDirection)
                   || sortDirection.ToUpperInvariant().StartsWith("ASC");
        }

        public static string ToCSVDateTime(DateTime? d)
        {
            return d.HasValue
                ? d.Value.ToString(AppConstants.DATA_EXPORT_DATETIME_FORMAT)
                : String.Empty;
        }

        public static string ToCSVDate(DateTime? d)
        {
            return d.HasValue
                ? d.Value.ToString(AppConstants.DATA_EXPORT_DATE_FORMAT)
                : String.Empty;
        }

        public static string ToCSVString(string s)
        {
            return s == null
                ? "\"\""
                : "\"" + s.Replace(Environment.NewLine, " ").Replace("\"", " ") + "\"";
        }

        public static string ToCSVBoolean(bool? b)
        {
            if (b == null) return String.Empty;
            return b.Value ? "TRUE" : "FALSE";
        }

        public static string ToCSVYN(bool? b)
        {
            if (b == null) return String.Empty;
            return b.Value ? "Y" : "N";
        }

        public static string ExecuteStringReplacements(string template, string firstName, string lastName, string appRootURL)
            => template.Replace("{{first-name}}", firstName)
                .Replace("{{last-name}}", lastName)
                .Replace("{{url}}", appRootURL);

        public static List<int> UserTenantIdList(UserDto user, TypeOfUserRole[] roleContextList)
        {
            if (user == null) return new List<int>();
            if (user.Roles == null)
                user.Roles = new List<UserRoleDto>();
            return user.Roles.Where(x => x.DeactivatedAt == null && roleContextList.Contains(x.TypeOfUserRole) 
                        && x.TenantId.HasValue)
                    .Select(x => x.TenantId.Value)
                    .Distinct().ToList();
        }

        public static List<int> UserMGIdList(UserDto user, TypeOfUserRole[] roleContextList = null)
        {
            if (user == null) return new List<int>();
            if (user.Roles == null)
                user.Roles = new List<UserRoleDto>();
            return user.Roles.Where(x => x.DeactivatedAt == null && roleContextList.Contains(x.TypeOfUserRole) 
                        && x.MGId.HasValue)
                    .Select(x => x.MGId.Value)
                    .Distinct().ToList();
        }

        public static List<int> UserCGIdList(UserDto user, TypeOfUserRole[] roleContextList = null)
        {
            if (user == null) return new List<int>();
            if (user.Roles == null)
                user.Roles = new List<UserRoleDto>();
            return user.Roles.Where(x => x.DeactivatedAt == null && roleContextList.Contains(x.TypeOfUserRole)
                        && x.CGId.HasValue)
                    .Select(x => x.CGId.Value)
                    .Distinct().ToList();
        }

        public static List<int> UserCategoryIdList(UserDto user, TypeOfUserRole[] roleContextList = null)
        {
            if (user == null) return new List<int>();
            if (user.Roles == null)
                user.Roles = new List<UserRoleDto>();
            return user.Roles.Where(x => x.DeactivatedAt == null && roleContextList.Contains(x.TypeOfUserRole)
                        && x.CategoryId.HasValue)
                    .Select(x => x.CategoryId.Value)
                    .Distinct().ToList();
        }

        public static List<int> UserSubcategoryIdList(UserDto user, TypeOfUserRole[] roleContextList = null)
        {
            if (user == null) return new List<int>();
            if (user.Roles == null)
                user.Roles = new List<UserRoleDto>();
            return user.Roles.Where(x => x.DeactivatedAt == null && roleContextList.Contains(x.TypeOfUserRole)
                        && x.SubcategoryId.HasValue)
                    .Select(x => x.SubcategoryId.Value)
                    .Distinct().ToList();
        }

        public static bool RequiresUserAccessConstraints(UserDto user)
        {
            if (user == null) return true;
            return !user.IsAdminUser && user.Id != AppConstants.RESOURCE_USER_ID;
        }

        public static List<string> GetTimezoneList()
            => TimeZoneInfo.GetSystemTimeZones().Select(x => x.Id).ToList();

        public static IList<int> SplitStringIdListIntoIntList(string stringToSplit)
        {
            var idList = new List<int>();
            if (!String.IsNullOrEmpty(stringToSplit))
            {
                string[] roleStringArray = stringToSplit.Split(",");
                foreach (var r in roleStringArray)
                {
                    idList.Add(int.Parse(r));
                };
            }
            return idList;
        }
    }
}
