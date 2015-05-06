using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using WMIT.DataServices.Model;
using WMIT.DataServices.Common;
using System.Runtime.ExceptionServices;
using System.Data.Entity.Infrastructure;
using System.Net;

namespace WMIT.DataServices.Controllers
{
    public class ODataController<TDbContext, TEntity> : ODataController
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

        protected TDbContext db = null;
        protected DbSet<TEntity> set = null;

        public ODataController(TDbContext dbContext)
        {
            db = dbContext;
            Initialize();
        }

        public ODataController()
        {
            db = new TDbContext();
            Initialize();
        }

        protected virtual void Initialize()
        {
            set = db.Set<TEntity>();
        }

        protected virtual async Task<bool> EntityExists(int id)
        {
            TEntity entity = await Entities.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
            return entity != null;
        }

        #endregion

        #region API

        // GET: api/entities
        [EnableQuery()]
        public virtual IHttpActionResult Get()
        {
            return Ok(Entities);
        }

        // GET: api/Contacts(5)
        [EnableQuery]
        public virtual IHttpActionResult Get([FromODataUri]int key)
        {
            var queryable = Entities.Where(e => e.Id == key);
            return Ok(SingleResult.Create(queryable));
        }

        // POST: api/entities
        [EnableQuery]
        public async Task<IHttpActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Fields like ModifiedAt/IsDeleted ... are resettet in this method.
            // We should implement an Authorize handler for this behavior which makes use of 
            // SystemFields/FieldAccess
            entity.SetCreationStatistics(User.Identity);

            db.Set<TEntity>().Add(entity);
            await db.SaveChangesAsync();

            entity = await Entities.Where(e => e.Id == entity.Id).SingleAsync();
            return Created(entity);
        }

        // PUT: api/Contacts(5)
        [EnableQuery]
        public async Task<IHttpActionResult> Put([FromODataUri]int key, TEntity entity)
        {
            Validate<TEntity>(entity);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await EntityExists(key))
            {
                return NotFound();
            }

            db.Entry(entity).Update().SetModificationStatistics(User.Identity);
            
            ExceptionDispatchInfo capturedException = null;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                if (!await EntityExists(key))
                {
                    return NotFound();
                }
                else
                {
                    capturedException.Throw();
                }
            }

            // Detach entity to ensure it will be refetched in later querying
            db.Entry(entity).State = EntityState.Detached;

            var queryable = Entities.Where(e => e.Id == entity.Id);
            return Updated(SingleResult.Create(queryable));
        }

        // DELETE: api/entities(5)
        [EnableQuery]
        public async Task<IHttpActionResult> Delete([FromODataUri]int key)
        {
            var entity = await Entities.SingleOrDefaultAsync(e => e.Id == key);

            if (entity == null)
            {
                return NotFound();
            }

            entity.IsDeleted = true;
            
            db.Entry(entity).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
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
