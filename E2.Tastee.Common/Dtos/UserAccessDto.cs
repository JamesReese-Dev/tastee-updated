using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace E2.Tastee.Common.Dtos
{
    public class UserAccessDto
    {
        public UserAccessDto()
        {
            CGIdsAsAdmin = new int[] { };
            CategoryIdsAsAdmin = new int[] { };
            SubcategoryIdsAsAdmin = new int[] { };
            CGIdsAsAgentManager = new int[] { };
            CategoryIdsAsAgentManager = new int[] { };
            SubcategoryIdsAsAgentManager = new int[] { };
            TenantIdsAsAdmin = new int[] { };
            MGIdsAsAdmin = new int[] { };
        }
        public int[] CGIdsAsAdmin { get; set; }
        public int[] CategoryIdsAsAdmin { get; set; }
        public int[] SubcategoryIdsAsAdmin { get; set; }
        public int[] CGIdsAsAgentManager { get; set; }
        public int[] CategoryIdsAsAgentManager { get; set; }
        public int[] SubcategoryIdsAsAgentManager { get; set; }
        public int[] TenantIdsAsAdmin { get; set; }
        public int[] MGIdsAsAdmin { get; set; }

        public int[] CGIds
        {
            get
            {
                return CGIdsAsAdmin.Union(CGIdsAsAgentManager).ToArray();
            }
        }
        public int[] CategoryIds
        {
            get
            {
                return CategoryIdsAsAdmin.Union(CategoryIdsAsAgentManager).ToArray();
            }
        }
        public int[] SubcategoryIds
        {
            get
            {
                return SubcategoryIdsAsAdmin.Union(SubcategoryIdsAsAgentManager).ToArray();
            }
        }
        public bool IsAgentManagerOnly
        {
            get
            {
                return CGIdsAsAdmin.Length == 0 && CategoryIdsAsAdmin.Length == 0 && SubcategoryIdsAsAdmin.Length == 0;
            }
        }
    }
}
