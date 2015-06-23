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
using WMIT.DataServices.Services;

namespace WMIT.DataServices.Controllers
{
    public class ODataController<TDbContext, TEntity> : ODataController
        where TDbContext : DbContext, new()
        where TEntity : class, IEntity
    {
        #region Internal

        protected IDataService<TEntity> service;

        public ODataController(IDataService<TEntity> service)
        {
            this.service = service;
        }

        public ODataController()
        {
            this.service = new EntityDataService<TDbContext, TEntity>(this.User.Identity);
        }
        
        protected virtual async Task<bool> EntityExists(int id)
        {
            TEntity entity = await service.Entities.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
            return entity != null;
        }

        #endregion

        #region API

        // GET: api/entities
        [EnableQuery()]
        public virtual IHttpActionResult Get()
        {
            return Ok(service.Entities);
        }

        // GET: api/Contacts(5)
        [EnableQuery]
        public virtual IHttpActionResult Get([FromODataUri]int key)
        {
            var queryable = service.Entities.Where(e => e.Id == key);
            return Ok(SingleResult.Create(queryable));
        }

        // POST: api/entities
        [EnableQuery]
        public virtual async Task<IHttpActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await service.Insert(entity);

            var queryable = service.Entities.Where(e => e.Id == entity.Id);
            return Content(HttpStatusCode.Created, SingleResult.Create(queryable));
        }

        // PUT: api/Contacts(5)
        [EnableQuery]
        public virtual async Task<IHttpActionResult> Put([FromODataUri]int key, TEntity entity)
        {
            Validate<TEntity>(entity);

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

            await service.Update(entity);

            var queryable = service.Entities.Where(e => e.Id == key);
            return Updated(SingleResult.Create(queryable));
        }

        // DELETE: api/entities(5)
        [EnableQuery]
        public virtual async Task<IHttpActionResult> Delete([FromODataUri]int key)
        {
            var entity = await service.Entities.SingleOrDefaultAsync(e => e.Id == key);

            if (entity == null)
            {
                return NotFound();
            }

            await service.Delete(entity);
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.service is IDisposable)
                    ((IDisposable)this.service).Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
