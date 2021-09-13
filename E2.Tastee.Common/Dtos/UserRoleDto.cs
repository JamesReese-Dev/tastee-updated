using Newtonsoft.Json;
using Serilog;
using E2.Tastee.Common.Extensions;
using System;

namespace E2.Tastee.Common
{
    [Serializable]
    public class UserRoleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TypeOfUserRole TypeOfUserRole { get; set; }
        public string RoleName { get { return TypeOfUserRole.Description(); } }
        public int? TenantId { get; set; }
        public string TenantName { get; set; }
        public int? MGId { get; set; }
        public string MGName { get; set; }
        public int? CGId { get; set; }
        public string CGName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public DateTime? DeactivatedAt { get; set; }

        public override string ToString()
        {
            return TypeOfUserRole.ToString();
        }
    }
}
