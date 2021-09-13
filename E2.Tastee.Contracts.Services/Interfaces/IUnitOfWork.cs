using System;

namespace E2.Tastee.Contracts.Services.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Begin();
        void Commit();
        void Rollback();
        bool IsInTransaction();
    }
}