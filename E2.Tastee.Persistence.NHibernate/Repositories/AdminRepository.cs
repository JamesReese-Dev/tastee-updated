using NHibernate;
using NHibernate.Linq;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace E2.Tastee.Persistence.NHibernate.Repositories
{
    public class AdminRepository : BaseRepository, IAdminRepository
    {
        private readonly IMapper _mapper;
        public AdminRepository(ISession nhSession, IMapper mapper) : base(nhSession)
        {
            _mapper = mapper;
        }
    }
}
