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
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public virtual async Task<IHttpActionResult> GetAll(ODataQueryOptions<TEntity> options)
        {
            var queryable = Entities.ApplyQuery(options);

            var entities = await queryable.ToListAsync();
            return Ok(entities);
        }

        // GET: api/Contacts(5)
        [EnableQuery]
        public virtual async Task<IHttpActionResult> GetEntity([FromODataUri]int key, ODataQueryOptions<TEntity> options)
        {
            var queryable = Entities.ApplyQuery(options);

            TEntity entity = await queryable.SingleOrDefaultAsync(e => e.Id == key);

            if (entity != null)
            {
                return Ok(entity);
            }
            else
            {
                return NotFound();
            }
        }

        // PUT: api/Contacts(5)
        [EnableQuery]
        public async Task<IHttpActionResult> PutEntity([FromODataUri]int key, TEntity entity, ODataQueryOptions<TEntity> options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != entity.Id)
            {
                return BadRequest();
            }

            if (!await EntityExists(key))
            {
                return NotFound();
            }

            db.Entry(entity).Update().SetModificationStatistics(User.Identity);

            TEntity updatedEntity = null;
            ExceptionDispatchInfo capturedException = null;

            try
            {
                await db.SaveChangesAsync();

                var queryable = Entities.ApplyQuery(options);
                updatedEntity = await queryable.SingleOrDefaultAsync(e => e.Id == key);
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

            return Updated(updatedEntity);
        }

        // POST: api/entities
        [EnableQuery]
        public async Task<IHttpActionResult> PostEntity(TEntity entity, ODataQueryOptions<TEntity> options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.SetCreationStatistics(User.Identity);

            db.Set<TEntity>().Add(entity);
            await db.SaveChangesAsync();

            var queryable = Entities.ApplyQuery(options);
            TEntity createdEntity = await queryable.SingleOrDefaultAsync(e => e.Id == entity.Id);

            return Created(createdEntity);
        }

        // DELETE: api/entities(5)
        [EnableQuery]
        public async Task<IHttpActionResult> DeleteEntity([FromODataUri]int key, ODataQueryOptions<TEntity> options)
        {
            var queryable = Entities.ApplyQuery(options);

            TEntity entity = await queryable.SingleOrDefaultAsync(e => e.Id == key);
            if (entity == null)
            {
                return NotFound();
            }

            entity.IsDeleted = true;
            db.Entry(entity).State = EntityState.Modified;

            await db.SaveChangesAsync();
            return Ok(entity);
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
