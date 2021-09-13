using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Common
{
    public class UserSearchCriteria: BaseSearchCriteria, ISortableCriteria
    {
        public UserSearchCriteria()
        {
            ExcludeSystemUser = true;
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string MothersMaidenName { get; set; }
        public Guid? PasswordResetToken { get; set; }
        public bool ExcludeSystemUser { get; set; }
        public bool? ActiveOnly { get; set; }
        public int? OrgId { get; set; }
        public int? ExceptUserId { get; set; }
        public IEnumerable<int> IdList { get; set; }
        public TypeOfUserRole[] MemberOfRoles { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
    }
}
