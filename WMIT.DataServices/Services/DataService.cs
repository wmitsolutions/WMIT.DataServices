using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Model;
using WMIT.DataServices.Common;

namespace WMIT.DataServices.Services
{
    public interface IDataService<T>
    {
        IQueryable<T> Entities { get; }

        Task Insert(T item);
        Task Update(T item);
        Task Delete(T item);
    }

    public class EntityDataService<TDbContext, TEntity> : IDataService<TEntity>, IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class, IEntity
    {
        #region Internal

        public virtual IQueryable<TEntity> Entities
        {
            get
            {
                return set.Where(e => !e.IsDeleted);
            }
        }

        protected TDbContext db = null;
        protected DbSet<TEntity> set = null;
        protected IIdentity identity = null;

        public EntityDataService(TDbContext dbContext, IIdentity identity)
        {
            Initialize(dbContext, identity);
        }

        public EntityDataService(IIdentity identity)
        {
            Initialize(null, identity);
        }

        protected virtual void Initialize(TDbContext dbContext, IIdentity identity)
        {
            this.db = dbContext ?? new TDbContext();
            this.set = this.db.Set<TEntity>();

            this.identity = identity;
        }

        #endregion

        public async Task Insert(TEntity item)
        {
            // TODO: Fields like ModifiedAt/IsDeleted ... are resettet in this method.
            // We should implement an Authorize handler for this behavior which makes use of 
            // SystemFields/FieldAccess
            item.SetCreationStatistics(this.identity);
            db.Set<TEntity>().Add(item);

            await db.SaveChangesAsync();
            db.Entry(item).State = EntityState.Detached;
        }

        public async Task Update(TEntity item)
        {
            db.Entry(item).Update().SetModificationStatistics(this.identity);

            await db.SaveChangesAsync();
            db.Entry(item).State = EntityState.Detached;
        }

        public async Task Delete(TEntity item)
        {
            db.Entry(item).Delete().SetModificationStatistics(this.identity);

            await db.SaveChangesAsync();
            db.Entry(item).State = EntityState.Detached;
        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }

        #endregion
    }
}
