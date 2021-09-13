// using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using E2.Tastee.Common.Extensions;

namespace E2.Tastee.Common
{
    public class UserDto : IIdentity
    {
        private IList<UserRoleDto> _roles;

        public IList<UserRoleDto> Roles
        {
            get { return _roles; }
            set
            {
                if (value != null)
                {
                    _roles = value.ToList();
                }
                else
                {
                    _roles = new List<UserRoleDto>();
                }
            }
        }

        public TypeOfUserRole[] RoleTypeIdList
        {
            get
            {
                if (_roles == null) return new TypeOfUserRole[] { };
                return _roles.Select(x => x.TypeOfUserRole).ToArray();
            }
        }
        public string[] RoleTypeNameList
        {
            get
            {
                if (_roles == null) return new string[] { };
                return _roles.Select(x => x.TypeOfUserRole.Description()).ToArray();
            }
        }

        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MothersMaidenName { get; set; }
        public string UserBloodType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Timezone { get; set; }
        public bool? MustChangePassword { get; set; }
        public string ResetPasswordToken { get; set; }
        public int FailedAttemptCount { get; set; }
        [JsonIgnore]
        public DateTime? TwoFactorAuthCodeExpiresAt { get; set; }
        [JsonIgnore]
        public string TwoFactorAuthCode { get; set; }
        [JsonIgnore]
        public string HashedPassword { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }
        [JsonIgnore]
        public DateTime? LastLoggedOnAt { get; set; }
        [JsonIgnore]
        public DateTime? ResetPasswordExpiresAt { get; set; }
        [JsonIgnore]
        public DateTime? LastLockedOutAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        [JsonIgnore]
        public int? DeactivatedByUserId { get; set; }
        // mapped/calculated/flattened properties
        public string Token { get; set; }
        public string LastLoggedOn
        {
            get
            {
                return LastLoggedOnAt.HasValue
                    ? LastLoggedOnAt.Value.ToString(AppConstants.DATE_FORMAT)
                    : "-";
            }
        }

        public bool PasswordResetRequested
        {
            get
            {
                return ResetPasswordExpiresAt.HasValue;
            }
        }

        public bool HasRegistrationToken { get; set; }

        public bool IsLockedOut
        {
            get
            {
                return LastLockedOutAt.HasValue;
            }
        }

        public bool IsAdminUser
        {
            get
            {
                if (Roles == null) return false;
                return Roles.Any(x => x.TypeOfUserRole == TypeOfUserRole.Administrator);
            }
        }

        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        [JsonIgnore]
        public string Name => Username;

        [JsonIgnore]
        public string AuthenticationType => AppConstants.ZB_AUTH_TYPE;

        [JsonIgnore]
        public bool IsAuthenticated => Id > 0;
    }
}
