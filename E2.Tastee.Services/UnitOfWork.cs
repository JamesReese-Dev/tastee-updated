using System;
using NHibernate;
using E2.Tastee.Contracts.Services.Interfaces;

namespace E2.Tastee.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private ITransaction _transaction = null;
        //private const string NoTransactionInProgress = "No transaction in progress";

        public UnitOfWork(ISession session)
        {
            _session = session;
        }

        //please do not remove, private constructor is needed
        private UnitOfWork() { }

        public void Begin()
        {
            if (_transaction == null)
            {
                _transaction = _session.BeginTransaction();
            }
            //else
            //{
            //    throw new InvalidOperationException("Transaction already in progress");
            //}
        }

        public void Commit()
        {
            if (_transaction == null || _transaction.WasCommitted)
            {
                return;
                // throw new InvalidOperationException(NoTransactionInProgress);
            }

            _session.Flush();
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public void Rollback()
        {
            if (_transaction == null)
            {
                return;
                // throw new InvalidOperationException(NoTransactionInProgress);
            }

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        public bool IsInTransaction()
        {
            return _transaction != null;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                if (_transaction.IsActive 
                    && !(_transaction.WasCommitted 
                         || _transaction.WasRolledBack) 
                    && _session.IsOpen 
                    && _session.IsConnected)
                {
                    _transaction.Rollback();
                }
                _transaction.Dispose();
            }

            _session.Dispose();
        }
    }
}