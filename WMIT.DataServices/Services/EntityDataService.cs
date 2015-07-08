using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Model;
using WMIT.DataServices.Common;
using WMIT.DataServices.Security;
using WMIT.DataServices.Visitors;

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
        public IPrincipal User { get; protected set; }

        public List<IEntityVisitor> Visitors { get; set; }

        public EntityDataService(TDbContext dbContext, IPrincipal user)
        {
            Initialize(dbContext, user);
        }

        public EntityDataService(IPrincipal user)
        {
            Initialize(null, user);
        }

        protected virtual void Initialize(TDbContext dbContext, IPrincipal user)
        {
            this.DbContext = dbContext ?? new TDbContext();
            this.Set = this.DbContext.Set<TEntity>();

            this.User = user;

            this.Visitors = new List<IEntityVisitor>();
            this.Visitors.Add(new AccessVisitor());
            this.Visitors.Add(new AutoValueVisitor());
        }

        #endregion

        #region Actions

        public virtual async Task Insert(TEntity item)
        {
            DbContext.Set<TEntity>().Add(item);
            var entry = DbContext.Entry<TEntity>(item);

            var context = new EntityContext();
            context.Entry = entry;
            context.User = this.User;
            context.Operation = EntityOperation.Insert;

            foreach (var visitor in this.Visitors)
                visitor.Visit(context);

            await DbContext.SaveChangesAsync();
            DbContext.Entry(item).State = EntityState.Detached;
        }

        public virtual async Task Update(TEntity item)
        {
            var entry = DbContext.Entry<TEntity>(item);
            entry.State = EntityState.Modified;

            var context = new EntityContext();
            context.Entry = entry;
            context.User = this.User;
            context.Operation = EntityOperation.Update;

            foreach (var visitor in this.Visitors)
                visitor.Visit(context);

            await DbContext.SaveChangesAsync();
            DbContext.Entry(item).State = EntityState.Detached;
        }

        public virtual async Task Delete(TEntity item)
        {
            var entry = DbContext.Entry<TEntity>(item);
            entry.State = EntityState.Unchanged;

            var context = new EntityContext();
            context.Entry = entry;
            context.User = this.User;
            context.Operation = EntityOperation.Delete;

            foreach (var visitor in this.Visitors)
                visitor.Visit(context);

            item.IsDeleted = true;
            entry.Property(p => p.IsDeleted).IsModified = true;

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
