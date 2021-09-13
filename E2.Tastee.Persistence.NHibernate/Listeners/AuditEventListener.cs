using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Listeners
{
    public class AuditEventListener : IPreInsertEventListener // IPreUpdateEventListener
    {
        //public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
        //{
        //    if (!(@event.Entity is UserManagedBase audit))
        //        return new Task<bool>(() => false);

        //    var time = DateTime.UtcNow;
        //    Set(@event.Persister, @event.State, "UpdatedAt", time);
        //    audit.UpdatedAt = time;
        //    return new Task<bool>(() => false);
        //}

        //public bool OnPreUpdate(PreUpdateEvent @event)
        //{
        //    if (!(@event.Entity is UserManagedBase audit))
        //        return false;

        //    var time = DateTime.UtcNow;
        //    Set(@event.Persister, @event.State, "UpdatedAt", time);
        //    audit.UpdatedAt = time;
        //    return false;
        //}

        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
            if (!(@event.Entity is UserManagedBase modelBase))
                return new Task<bool>(() => false);

            var time = DateTime.UtcNow;
            Set(@event.Persister, @event.State, "CreatedAt", time);
            // Set(@event.Persister, @event.State, "UpdatedAt", time);
            modelBase.CreatedAt = time;
            // modelBase.UpdatedAt = time;
            return new Task<bool>(() => false);
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            if (!(@event.Entity is UserManagedBase modelBase))
                return false;

            var time = DateTime.UtcNow;
            Set(@event.Persister, @event.State, "CreatedAt", time);
            // Set(@event.Persister, @event.State, "UpdatedAt", time);
            modelBase.CreatedAt = time;
            // modelBase.UpdatedAt = time;
            return false;
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}