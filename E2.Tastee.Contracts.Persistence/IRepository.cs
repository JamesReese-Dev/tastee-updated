using E2.Tastee.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E2.Tastee.Contracts.Persistence
{
    public interface IRepository
    {
        T Create<T>(T t);
        // hangs...
        //Task<T> CreateAsync<T>(T t);
        void Update<T>(T t);
        Task UpdateAsync<T>(T entity);
        void HardDelete<T>(T t);
        Task HardDeleteAsync<T>(T entity);
        T Load<T>(object id);
        T Get<T>(object id);
        Task<T> GetAsync<T>(int id);
        Task<bool> WouldBeDuplicateSimpleDto<T>(SimpleDto dto) where T : ISoftDeletableDto, ISimpleDto;
        IList<SimpleDto> FindLightList<T>(string name = null, int? maxResults = null) where T : ISimpleDto;
        Task<List<SimpleDto>> FindLightListAsync<T>(string name = null, int? maxResults = null) where T : ISimpleDto;
        Task<PaginatedList<SoftDeletableSimpleDto>> FindPaginatedLightListAsync<T>(SimpleCriteria criteria) where T : ISoftDeletableDto, ISimpleDto;
        Task<List<SimpleDto>> FindLightWithActiveOnlyAsync<T>(bool activeOnly = true) where T : ISoftDeletableDto, ISimpleDto;
    }
}
