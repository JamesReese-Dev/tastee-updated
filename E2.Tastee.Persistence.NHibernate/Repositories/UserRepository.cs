using NHibernate;
using NHibernate.Linq;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E2.Tastee.Persistence.NHibernate.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(ISession nhSession) : base(nhSession)
        {
        }

        public async Task<string> GetDefaultSubdomain(int userId)
        {
            return await _session.CreateSQLQuery(@"SELECT TOP 1 (tx.Subdomain)
                 FROM dbo.UserRoles r INNER JOIN dbo.Tenants tx ON tx.Id = r.TenantId
                    LEFT JOIN dbo.MGs mx ON mx.TenantId = tx.Id
                    LEFT JOIN dbo.CGs cx ON cx.MGid = mx.Id
                    LEFT JOIN dbo.Categories catx ON catx.CGId = cx.Id
                    LEFT JOIN dbo.Subcategories scatx ON scatx.CategoryId = catx.Id
                 WHERE r.UserId = :userId
                    AND r.DeactivatedAt IS NULL
                 ORDER BY CASE
                    WHEN r.TenantId IS NOT NULL THEN 1
                    WHEN r.MGId IS NOT NULL THEN 2
                    WHEN r.CGId IS NOT NULL THEN 3
                    WHEN r.CategoryId IS NOT NULL THEN 4
                    ELSE 5
                    END")
                .SetInt32("userId", userId)
                .UniqueResultAsync<string>();
        }

        private IQueryable<User> resolveUserQuery(UserSearchCriteria criteria, UserDto currentUser)
        {
            var query = _session.Query<User>();
            if (criteria.ActiveOnly ?? false)
            {
                query = query.Where(x => x.DeactivatedAt == null);
            }
            if (criteria.ExcludeSystemUser)
            {
                query = query.Where(x => x.Id > 0); // resource account and fake, test user accounts
            }
            if (!String.IsNullOrEmpty(criteria.Username))
            {
                query = query.Where(x => x.Username == criteria.Username);
            }
            if (!String.IsNullOrEmpty(criteria.Email))
            {
                query = query.Where(x => x.Email == criteria.Email);
            }
            if (!String.IsNullOrEmpty(criteria.Name))
            {
                query = query.Where(x => (x.FirstName + " " + x.LastName).Contains(criteria.Name));
            }
            if(!String.IsNullOrEmpty(criteria.MothersMaidenName))
            {
                query = query.Where(x => x.MothersMaidenName == criteria.MothersMaidenName);
            }
            if (criteria.Id.HasValue)
            {
                query = query.Where(x => x.Id == criteria.Id.Value);
            }
            if (criteria.ExceptUserId.HasValue)
            {
                query = query.Where(x => x.Id != criteria.ExceptUserId.Value);
            }
            if (criteria.IdList != null && criteria.IdList.Any())
            {
                int[] IdArray = criteria.IdList.ToArray();
                query = query.Where(x => IdArray.Contains(x.Id));
            }
            if (criteria.PasswordResetToken.HasValue)
            {
                query = query.Where(x => x.ResetPasswordToken == criteria.PasswordResetToken.Value);
            }
            if (criteria.MemberOfRoles != null && criteria.MemberOfRoles.Any())
            {
                query = query.Where(x => x.Roles.Any(y => criteria.MemberOfRoles.Contains(y.TypeOfUserRole)));
            }
            if (!String.IsNullOrEmpty(criteria.SortField))
            {
                bool sortAscending = AppHelpers.IsSortAscending(criteria.SortDirection);
                switch(criteria.SortField.ToUpperInvariant())
                {
                    case "NAME":
                        query = sortAscending
                            ? query.OrderBy(x => (x.FirstName + " " + x.LastName))
                            : query.OrderByDescending(x => (x.FirstName + " " + x.LastName));
                        break;
                    // TODO: add other sort options here...
                    default:
                        query = sortAscending
                                ? query.OrderBy(x => x.Id)
                                : query.OrderByDescending(x => x.Id);
                        break;
                }
            }
            return query;
        }

        public async Task<PaginatedList<User>> FindPaginatedUsersAsync(UserSearchCriteria criteria, UserDto currentUser)
        {
            var (pageNumber, pageSize) = ensureValidPagination(criteria.PageNumber, criteria.MaxResults);
            var query = resolveUserQuery(criteria, currentUser);
            var count = await query.CountAsync();
            var list = await query.Fetch(x => x.Roles)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return new PaginatedList<User>()
            {
                Items = list,
                PageNumber = pageNumber,
                TotalCount = count
            };
        }

        public Task<List<User>> FindUsersAsync(UserSearchCriteria criteria, UserDto currentUser)
        {
            return resolveUserQuery(criteria, currentUser)
                .Fetch(x => x.Roles)
                //.OrderBy(x => x.LastName)
                //.ThenBy(x => x.FirstName)
                .Take(criteria.MaxResults)
                .ToListAsync();
        }

        public async Task HardDeleteUser(int userId)
        {
/*
select concat('UPDATE dbo.', object_name(object_id), ' SET ', name, ' = :resourceAccountUserId WHERE ', name, ' = :userId;') as s
from sys.columns
where name like '%userid' 
order by 1
*/

            string sql =
@"UPDATE dbo.UserRoles SET CreatedByUserId = :resourceAccountUserId WHERE CreatedByUserId = :userId;
UPDATE dbo.UserRoles SET DeactivatedByUserId = :resourceAccountUserId WHERE DeactivatedByUserId = :userId;
UPDATE dbo.Users SET CreatedByUserId = :resourceAccountUserId WHERE CreatedByUserId = :userId;
UPDATE dbo.Users SET DeactivatedByUserId = :resourceAccountUserId WHERE DeactivatedByUserId = :userId;
UPDATE dbo.Users SET LastUpdatedByUserId = :resourceAccountUserId WHERE LastUpdatedByUserId = :userId;

DELETE FROM dbo.UserRoles WHERE UserId = :userId;
DELETE FROM dbo.Users WHERE Id = :userId;";
            await _session.CreateSQLQuery(sql)
                .SetInt32("userId", userId)
                .SetInt32("resourceAccountUserId", AppConstants.RESOURCE_USER_ID)
                .ExecuteUpdateAsync();
        }
    }
}
