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

        // PUT: api/Contacts(5)
        [EnableQuery]
        public async Task<IHttpActionResult> Put([FromODataUri]int key, TEntity entity)
        {
            if (!ModelState.IsValid || key != entity.Id)
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

                // Detach entity to ensure it will be refetched in later querying
                db.Entry(entity).State = EntityState.Detached;

                var queryable = Entities.Where(e => e.Id == entity.Id);
                return Updated(SingleResult.Create(queryable));
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

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        // POST: api/entities
        [EnableQuery]
        public async Task<IHttpActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            entity.SetCreationStatistics(User.Identity);

            db.Set<TEntity>().Add(entity);
            await db.SaveChangesAsync();

            var queryable = Entities.Where(e => e.Id == entity.Id);
            return Created(SingleResult.Create(queryable));
        }

        //// DELETE: api/entities(5)
        //[EnableQuery]
        //public async Task<IHttpActionResult> Delete([FromODataUri]int key, ODataQueryOptions<TEntity> options)
        //{
        //    var queryable = Entities.ApplyQuery(options);

        //    TEntity entity = await queryable.SingleOrDefaultAsync(e => e.Id == key);
        //    if (entity == null)
        //    {
        //        return NotFound();
        //    }

        //    entity.IsDeleted = true;
        //    db.Entry(entity).State = EntityState.Modified;

        //    await db.SaveChangesAsync();
        //    return Ok(entity);
        //}

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
