using System;
using System.Collections.Generic;


namespace E2.Tastee.Models
{
    public class User : DeactivatedModelBase
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string MobilePhoneNumber { get; set; }
        public virtual string Timezone { get; set; }
        public virtual string HashedPassword { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual string TwoFactorAuthCode { get; set; }
        public virtual DateTime? TwoFactorAuthCodeExpiresAt { get; set; }
        public virtual Guid? ResetPasswordToken { get; set; }
        public virtual DateTime? ResetPasswordRequestedAt { get; set; }
        public virtual DateTime? ResetPasswordExpiresAt { get; set; }
        public virtual DateTime? LastLockedOutAt { get; set; }
        public virtual bool MustChangePassword { get; set; }
        public virtual int FailedAttemptCount { get; set; }
        public virtual DateTime? PasswordLastChangedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual int? LastUpdatedByUserId { get; set; }
        public virtual DateTime? LastLoggedOnAt { get; set; }
        public virtual int? ZipCode { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual string MothersMaidenName { get; set; }

        public virtual string UserBloodType { get; set; }
    }
}
