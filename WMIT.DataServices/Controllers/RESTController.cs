using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WMIT.DataServices.Common;
using WMIT.DataServices.Model;

namespace WMIT.DataServices.REST
{
    public class RESTController<TDbContext, TEntity> : ApiController
        where TDbContext : DbContext, new()
        where TEntity : class, IEntity
    {
        #region Internal

        protected virtual IQueryable<TEntity> Entities
        {
            get
            {
                return set.Where(e => !e.IsDeleted);
            }
        }

        protected TDbContext db;
        protected DbSet<TEntity> set;

        public RESTController(TDbContext dbContext)
        {
            db = dbContext;
            Initialize();
        }

        public RESTController()
        {
            db = new TDbContext();
            Initialize();
        }

        protected virtual void Initialize()
        {
            set = db.Set<TEntity>();

            // Change Detection is not used in our web services,
            // we can disable automatic change detection for the entire controller
            // to boost the performance when executing FindAsync
            // Ref: http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id
            db.Configuration.AutoDetectChangesEnabled = false;
        }

        protected virtual async Task<bool> EntityExists(int id)
        {
            TEntity entity = await Entities.SingleOrDefaultAsync(e => e.Id == id);
            return entity != null;
        }

        #endregion

        #region API

        // GET: api/entities
        public virtual async Task<List<TEntity>> GetAll()
        {
            return await Entities.ToListAsync();
        }

        // GET: api/entities/5
        public virtual async Task<IHttpActionResult> GetEntity(int id)
        {
            TEntity entity = await Entities.SingleOrDefaultAsync(e => e.Id == id);

            if (entity != null)
            {
                return Ok(entity);
            }
            else
            {
                return NotFound();
            }
        }

        // PUT: api/Contacts/5
        public virtual async Task<IHttpActionResult> PutEntity(int id, TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Id)
            {
                return BadRequest();
            }

            if (!await EntityExists(id))
            {
                return NotFound();
            }

            db.Entry(entity).State = EntityState.Modified;

            //db.Entry(entity).Update
            //db.Entry(entity).Update().SetModificationStatistics(); // TODO: implement

            TEntity updatedEntity = null;
            ExceptionDispatchInfo capturedException = null;

            try
            {
                await db.SaveChangesAsync();
                updatedEntity = await db.Set<TEntity>().FindAsync(id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                if (!await EntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    capturedException.Throw();
                }
            }

            return Ok(updatedEntity);
        }

        // POST: api/entities
        public virtual async Task<IHttpActionResult> PostEntity(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //entity.SetCreationStatistics(); // TODO: implement
            db.Set<TEntity>().Add(entity);
            int result = await db.SaveChangesAsync();

            if (result == 1)
            {
                return CreatedAtRoute("DefaultApi", new { id = entity.Id }, entity);
            }
            else
            {
                throw new NoRowsAffectedException();
            }
        }

        // DELETE: api/entities/5
        public virtual async Task<IHttpActionResult> DeleteEntity(int id)
        {
            TEntity entity = await db.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.IsDeleted = true;
            db.Entry(entity).State = EntityState.Modified;

            int result = await db.SaveChangesAsync();

            if (result == 1)
            {
                return Ok(entity);
            }
            else
            {
                throw new NoRowsAffectedException();
            }
        }

        #endregion

        #region IDisposable implementation
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
