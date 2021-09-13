using System;

namespace E2.Tastee.Models
{
    public class UserAuthenticationAttempt : ModelBase
    {
        public virtual string Username { get; set; }
        public virtual byte[] IP { get; set; }
        public virtual DateTime OccurredAt { get; set; }
        public virtual bool WasSuccessful { get; set; }
        public virtual string FailureReason { get; set; }
    }
}
