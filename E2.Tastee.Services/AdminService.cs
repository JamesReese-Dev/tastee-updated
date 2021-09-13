using AutoMapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E2.Tastee.Common;
using E2.Tastee.Common.Dtos;
using E2.Tastee.Common.Extensions;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Contracts.Services.Dtos;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.Models;
using static E2.Tastee.Common.AppHelpers;

namespace E2.Tastee.Services
{
    public class AdminService : BaseDataService, IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IReferenceRepository referenceRepository, IMapper mapper,
            IUnitOfWork unitOfWork, AppSettings appSettings, IAdminRepository adminRepository)
            : base(referenceRepository, mapper, appSettings, unitOfWork)
        {
            _adminRepository = adminRepository;
        }
    }
}
