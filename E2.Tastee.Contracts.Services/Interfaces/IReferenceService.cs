using E2.Tastee.Common;
using E2.Tastee.Contracts.Services.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Contracts.Services.Interfaces
{
    public interface IReferenceService
    {
        Task<List<SimpleDto>> GetListAsync<T>() where T : ISimpleDto;
        Task<List<SimpleDto>> GetLightActiveListAsync<T>(bool activeOnly = true) where T : ISoftDeletableDto, ISimpleDto;
        Task<PaginatedList<SoftDeletableSimpleDto>> GetPaginatedListAsync<T>(SimpleCriteria criteria) where T : ISoftDeletableDto, ISimpleDto;
        Task<ISimpleDto> GetAsync<T>(int id) where T : ISimpleDto;
        Task ToggleActiveAsync<T>(int id, UserDto currentUser) where T : ISoftDeletableDto;
        Task<SoftDeletableSimpleDto> SaveAsync<T>(TrackedSimpleDto dto, UserDto currentUser) where T : ISoftDeletableDto, ITrackedSimpleDto;
    }
}
