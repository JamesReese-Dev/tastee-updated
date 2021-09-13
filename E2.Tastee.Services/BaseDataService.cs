using AutoMapper;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Contracts.Services.Dtos;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using E2.Tastee.Contracts.Services.Interfaces;
using System.Linq;

namespace E2.Tastee.Services
{
    public class BaseDataService
    {
        protected readonly IReferenceRepository _referenceRepository;
        protected readonly IMapper _mapper;
        protected readonly AppSettings _appSettings;
        protected IUnitOfWork _unitOfWork;

        public BaseDataService(IReferenceRepository referenceRepository, IMapper mapper, AppSettings appSettings, IUnitOfWork unitOfWork)
        {
            _referenceRepository = referenceRepository;
            _mapper = mapper;
            _appSettings = appSettings;
            _unitOfWork = unitOfWork;
        }

        protected async Task toggleModelActive<T>(int id, int userId) where T : ISoftDeletableDto
        {
            try
            {
                _unitOfWork.Begin();
                T model = await _referenceRepository.GetAsync<T>(id);
                if (model == null)
                    throw new ApplicationException(AppConstants.ErrorMessages.ItemNotFound);
                if (model.DeactivatedAt == null)
                {
                    model.DeactivatedAt = DateTime.UtcNow;
                    model.DeactivatedByUserId = userId;
                }
                else
                {
                    model.DeactivatedAt = null;
                    model.DeactivatedByUserId = null;
                }
                await _referenceRepository.UpdateAsync(model);

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Log.Error(ex, $"Error during {typeof(T).FullName} toggle active");
                throw;
            }
        }

        protected void EnsureValidPagination(PagingCriteria criteria)
        {
            if (criteria.PageNumber <= 0)
                criteria.PageNumber = 1;
            if (criteria.MaxResults <= 0)
                criteria.MaxResults = _appSettings.DefaultPageSizeInRows;
        }
    }
}
