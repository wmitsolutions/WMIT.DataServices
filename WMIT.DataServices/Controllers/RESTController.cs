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
using WMIT.DataServices.Model;

namespace WMIT.DataServices.REST
{
    public class RESTController<TDbContext, TEntity> : ApiController
        where TDbContext : DbContext, new()
        where TEntity : class, IEntity
    {
        protected TDbContext db;
        protected DbSet<TEntity> set;

        public RESTController()
        {
            db = new TDbContext();
            set = db.Set<TEntity>();

            // Change Detection is not used in our web services,
            // we can disable automatic change detection for the entire controller
            // to boost the performance when executing FindAsync
            // Ref: http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id
            db.Configuration.AutoDetectChangesEnabled = false;
        }

        // GET: api/entities
        public IQueryable<TEntity> GetAll()
        {
            return db.Set<TEntity>().Where(e => !e.IsDeleted);
        }

        // GET: api/entities/5
        public async Task<IHttpActionResult> GetEntity(int id)
        {
            TEntity entity = await db.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PUT: api/Contacts/5
        public async Task<IHttpActionResult> PutEntity(int id, TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != entity.Id)
            {
                return BadRequest();
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
        public async Task<IHttpActionResult> PostEntity(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //entity.SetCreationStatistics(); // TODO: implement
            db.Set<TEntity>().Add(entity);
            await db.SaveChangesAsync();

            //var createdEntity = await db.Set<TEntity>().FindAsync(entity.Id);

            return CreatedAtRoute("DefaultApi", new { id = entity.Id }, entity);
        }

        // DELETE: api/entities/5
        public async Task<IHttpActionResult> DeleteEntity(object id)
        {
            TEntity entity = await db.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.IsDeleted = true;
            await db.SaveChangesAsync();

            return Ok(entity);
        }

        private async Task<bool> EntityExists(params object[] keyValues)
        {
            TEntity entity = await db.Set<TEntity>().FindAsync(keyValues);
            return entity != null;
        }

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
