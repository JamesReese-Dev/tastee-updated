using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Transform;
using E2.Tastee.Common.Dtos;

namespace E2.Tastee.Persistence.NHibernate.Repositories
{
    public class ReferenceRepository: BaseRepository, IReferenceRepository
    {
        public ReferenceRepository(ISession nhSession) : base(nhSession)
        {
        }
    }
}
