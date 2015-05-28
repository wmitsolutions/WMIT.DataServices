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
    public class EntityDataService<TDbContext, TEntity> : IDataService<TEntity>, IDisposable
        where TDbContext : DbContext, new()
        where TEntity : class, IEntity
    {
        #region Internal

        public virtual IQueryable<TEntity> Entities
        {
            get
            {
                return Set.Where(e => !e.IsDeleted);
            }
        }

        public TDbContext DbContext { get; protected set; }
        public DbSet<TEntity> Set { get; protected set; }
        public IIdentity Identity { get; protected set; }

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
            this.DbContext = dbContext ?? new TDbContext();
            this.Set = this.DbContext.Set<TEntity>();

            this.Identity = identity;
        }

        #endregion

        #region Actions

        public virtual async Task Insert(TEntity item)
        {
            // TODO: Fields like ModifiedAt/IsDeleted ... are resettet in this method.
            // We should implement an Authorize handler for this behavior which makes use of 
            // SystemFields/FieldAccess
            item.SetCreationStatistics(this.Identity);
            DbContext.Set<TEntity>().Add(item);

            await DbContext.SaveChangesAsync();
            DbContext.Entry(item).State = EntityState.Detached;
        }

        public virtual async Task Update(TEntity item)
        {
            DbContext.Entry(item).Update().SetModificationStatistics(this.Identity);

            await DbContext.SaveChangesAsync();
            DbContext.Entry(item).State = EntityState.Detached;
        }

        public virtual async Task Delete(TEntity item)
        {
            DbContext.Entry(item).Delete().SetModificationStatistics(this.Identity);

            await DbContext.SaveChangesAsync();
            DbContext.Entry(item).State = EntityState.Detached;
        }

        #endregion

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
                DbContext.Dispose();
            }
        }

        #endregion
    }
}
