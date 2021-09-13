using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class UserMap : DeactivatedModelBaseMap<User>
    {
        public UserMap() : base("Users")
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Username);
            Map(x => x.Email);
            Map(x => x.MobilePhoneNumber).Nullable();
            Map(x => x.Timezone).Nullable();
            Map(x => x.HashedPassword);
            Map(x => x.PasswordSalt);
            Map(x => x.TwoFactorAuthCode).Nullable();
            Map(x => x.TwoFactorAuthCodeExpiresAt).Nullable();
            Map(x => x.ResetPasswordToken).Nullable();
            Map(x => x.ResetPasswordRequestedAt).Nullable();
            Map(x => x.ResetPasswordExpiresAt).Nullable();
            Map(x => x.LastLockedOutAt).Nullable();
            Map(x => x.MustChangePassword).Nullable();
            Map(x => x.FailedAttemptCount);
            Map(x => x.PasswordLastChangedAt).Nullable();
            Map(x => x.UpdatedAt);
            Map(x => x.LastLoggedOnAt).Nullable();
            Map(x => x.LastUpdatedByUserId).Nullable();
            Map(x => x.ZipCode).Nullable();
            HasMany(x => x.Roles)
                .Where("DeactivatedAt IS NULL")
                .KeyColumn("UserId")
                .Cascade.None()
                .Inverse();
            Map(x => x.MothersMaidenName).Nullable();
            Map(x => x.UserBloodType).Nullable();
        }
    }
}
