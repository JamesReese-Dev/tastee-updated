using FluentNHibernate.Mapping;
using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class UserAuthenticationAttemptMap : ModelBaseMap<UserAuthenticationAttempt>
    {
        public UserAuthenticationAttemptMap() : base("UserAuthenticationAttempts")
        {
            Map(x => x.Username);
            Map(x => x.IP);
            Map(x => x.OccurredAt);
            Map(x => x.WasSuccessful);
            Map(x => x.FailureReason).Nullable();
        }
    }
}
