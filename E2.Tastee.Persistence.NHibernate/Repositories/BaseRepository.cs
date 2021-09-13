using NHibernate;
using NHibernate.Linq;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E2.Tastee.Common.Dtos;
using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Repositories
{
    public class BaseRepository: IRepository
    {
        protected readonly ISession _session;

        public BaseRepository(ISession session)
        {
            _session = session;
        }

        protected Tuple<int, int> ensureValidPagination(int pageNumber, int maxResults)
        {
            if (pageNumber <= 0)
                pageNumber = 1;
            if (maxResults <= 0)
                throw new ApplicationException("Invalid max results specified, must be >= 0");
            return new Tuple<int, int>(pageNumber, maxResults);
        }

        public T Create<T>(T t)
        {
            _session.Save(t);
            return t;
        }

        public void Update<T>(T t)
        {
            _session.Update(t);
        }

        public async Task UpdateAsync<T>(T entity)
        {
            await _session.UpdateAsync(entity);
        }

        public void HardDelete<T>(T t)
        {
            _session.Delete(t);
        }

        public T Load<T>(object id)
        {
            return _session.Load<T>(id);
        }

        public T Get<T>(object id)
        {
            return _session.Get<T>(id);
        }

        public Task<T> GetAsync<T>(int id)
        {
            return _session.GetAsync<T>(id);
        }

        public async Task HardDeleteAsync<T>(T entity)
        {
            await _session.DeleteAsync(entity);
        }

        public async Task<bool> WouldBeDuplicateSimpleDto<T>(SimpleDto dto) where T : ISoftDeletableDto, ISimpleDto
        {
            var duplicateCount = await _session.Query<T>()
                .Where(x => x.Id != dto.Id
                    && x.Name == dto.Name
                    && x.DeactivatedAt == null)
                .CountAsync();
            return duplicateCount > 0;
        }

        private IQueryable<SimpleDto> resolveLightListQuery<T>(string name, int? maxResults = null) where T : ISimpleDto
        {
            var query = _session.Query<T>();
            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            return query.Select(x => new SimpleDto()
            {
                Id = x.Id,
                Name = x.Name
            })
            .OrderBy(x => x.Name)
            .Take(maxResults.HasValue ? maxResults.Value : AppConstants.UNBOUNDED_RESULT_ROW_COUNT);
        }

        private IQueryable<SoftDeletableSimpleDto> resolveSoftDeletableLightListQuery<T>(string name, bool? activeOnly, 
            bool sortAscending, int? maxResults = null) where T : ISoftDeletableDto, ISimpleDto
        {
            var query = _session.Query<T>();
            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (activeOnly ?? false)
            {
                query = query.Where(x => x.DeactivatedAt == null);
            }
            var resultQuery = query.Select(x => new SoftDeletableSimpleDto()
            {
                Id = x.Id,
                Name = x.Name,
                DeactivatedAt = x.DeactivatedAt
            });
            if (sortAscending)
            {
                resultQuery = resultQuery.OrderBy(x => x.Name);
            } else
            {
                resultQuery = resultQuery.OrderByDescending(x => x.Name);
            }
            return resultQuery.Take(maxResults.HasValue ? maxResults.Value : AppConstants.UNBOUNDED_RESULT_ROW_COUNT);
        }

        public async Task<PaginatedList<SoftDeletableSimpleDto>> FindPaginatedLightListAsync<T>(SimpleCriteria criteria) where T : ISoftDeletableDto, ISimpleDto
        {
            var (pageNumber, pageSize) = ensureValidPagination(criteria.PageNumber, criteria.MaxResults);
            var query = resolveSoftDeletableLightListQuery<T>(criteria.Name, criteria.ActiveOnly,
                                AppHelpers.IsSortAscending(criteria.SortDirection));
            var list = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return new PaginatedList<SoftDeletableSimpleDto>()
            {
                Items = list,
                PageNumber = pageNumber,
                TotalCount = query.Count()
            };
        }

        public Task<List<SimpleDto>> FindLightListAsync<T>(string name, int? maxResults = null) where T : ISimpleDto
        {
            return resolveLightListQuery<T>(name, maxResults)
                .ToListAsync();
        }

        public IList<SimpleDto> FindLightList<T>(string name, int? maxResults = null) where T : ISimpleDto
        {
            return resolveLightListQuery<T>(name, maxResults)
                .ToList();
        }

        public Task<List<SimpleDto>> FindLightWithActiveOnlyAsync<T>(bool activeOnly = false) where T : ISoftDeletableDto, ISimpleDto
        {
            var query = _session.Query<T>();
            if (activeOnly)
            {
                query = query.Where(x => x.DeactivatedAt == null);
            }
            var resultQuery = query.Select(x => new SimpleDto()
            {
                Id = x.Id,
                Name = x.Name,
            })
            .OrderBy(x => x.Name);

            return resultQuery.ToListAsync();
        }
    }
}
