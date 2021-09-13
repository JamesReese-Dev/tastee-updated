using AutoMapper;
using Serilog;
using E2.Tastee.Common;
using E2.Tastee.Common.Extensions;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Contracts.Services.Dtos;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E2.Tastee.Common.Dtos;
using System.Linq;

namespace E2.Tastee.Services
{
    public class ReferenceService : BaseDataService, IReferenceService
    {
        public ReferenceService(IReferenceRepository referenceRepository, IMapper mapper, AppSettings appSettings,
            IUnitOfWork unitOfWork) : base(referenceRepository, mapper, appSettings, unitOfWork)
        {
        }

        // TODO: cache these lists
        public async Task<List<SimpleDto>> GetLightActiveListAsync<T>(bool activeOnly = true) where T : ISoftDeletableDto, ISimpleDto
            => await _referenceRepository.FindLightWithActiveOnlyAsync<T>(activeOnly);

        private async Task<SoftDeletableSimpleDto> saveSimpleItemAsync<T>(TrackedSimpleDto dto) where T : ISoftDeletableDto, ITrackedSimpleDto
        {
            try
            {
                _unitOfWork.Begin();
                bool addingNew = dto.Id <= 0;
                T model = addingNew
                    ? Activator.CreateInstance<T>()
                    : await _referenceRepository.GetAsync<T>(dto.Id);
                if (!addingNew && await _referenceRepository.WouldBeDuplicateSimpleDto<T>(new SimpleDto() { Id = dto.Id, Name = dto.Name }))
                {
                    throw new ApplicationException(AppConstants.ErrorMessages.WouldBeDuplicate);
                }
                model.Name = dto.Name;
                model.CreatedByUserId = dto.CreatedByUserId;
                if (addingNew)
                {
                    model = _referenceRepository.Create(model);
                }
                else
                {
                    await _referenceRepository.UpdateAsync(model);
                }
                _unitOfWork.Commit();
                return _mapper.Map<T, SoftDeletableSimpleDto>(model);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, $"Error during {typeof(T).FullName} save");
                throw;
            }
        }

        public Task<PaginatedList<SoftDeletableSimpleDto>> GetPaginatedListAsync<T>(SimpleCriteria criteria) where T : ISoftDeletableDto, ISimpleDto
            => _referenceRepository.FindPaginatedLightListAsync<T>(criteria);

        public Task<List<SimpleDto>> GetListAsync<T>() where T : ISimpleDto
            => _referenceRepository.FindLightListAsync<T>();

        public async Task<ISimpleDto> GetAsync<T>(int id) where T : ISimpleDto
            => await _referenceRepository.GetAsync<T>(id);

        public Task ToggleActiveAsync<T>(int id, UserDto currentUser) where T : ISoftDeletableDto
        {
            if (!currentUser.IsAdminUser)
                throw new ApplicationException(AppConstants.ErrorMessages.AccessDenied);
            return toggleModelActive<T>(id, currentUser.Id.Value);
        }

        public Task<SoftDeletableSimpleDto> SaveAsync<T>(TrackedSimpleDto dto, UserDto currentUser) where T : ISoftDeletableDto, ITrackedSimpleDto
        {
            if (!currentUser.IsAdminUser)
                throw new ApplicationException(AppConstants.ErrorMessages.AccessDenied);
            return saveSimpleItemAsync<T>(dto);
        }
    }
}
