using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WMIT.DataServices.Common;
using WMIT.DataServices.Model;
using WMIT.DataServices.Services;

namespace WMIT.DataServices.Controllers
{
    public class RESTController<TDbContext, TEntity> : ApiController
        where TDbContext : DbContext, new()
        where TEntity : class, IEntity
    {
        #region Internal

        protected IDataService<TEntity> service;

        public RESTController(IDataService<TEntity> service)
        {
            this.service = service;
        }

        public RESTController()
        {
            this.service = new EntityDataService<TDbContext, TEntity>(this.User);
        }
        
        protected virtual async Task<bool> EntityExists(int id)
        {
            TEntity entity = await service.Entities.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
            return entity != null;
        }

        #endregion

        #region API

        // GET: api/entities
        public virtual async Task<IHttpActionResult> GetAll()
        {
            var entities = await service.Entities.ToListAsync();
            return Ok(entities);
        }

        // GET: api/entities/5
        public virtual async Task<IHttpActionResult> GetEntity(int id)
        {
            TEntity entity = await service.Entities.SingleOrDefaultAsync(e => e.Id == id);

            if (entity != null)
            {
                return Ok(entity);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: api/entities
        public virtual async Task<IHttpActionResult> PostEntity(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await service.Insert(entity);

            var createdEntity = await service.Entities.SingleOrDefaultAsync(e => e.Id == entity.Id);
            return CreatedAtRoute("DefaultApi", new { id = entity.Id }, createdEntity);
        }

        // PUT: api/Contacts/5
        public virtual async Task<IHttpActionResult> PutEntity(int id, TEntity entity)
        {
            Validate<TEntity>(entity);

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

            await service.Update(entity);

            var updatedEntity = await service.Entities.SingleOrDefaultAsync(e => e.Id == id);
            return Ok(updatedEntity);
        }

        // DELETE: api/entities/5
        public virtual async Task<IHttpActionResult> DeleteEntity(int id)
        {
            TEntity entity = await service.Entities.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            await service.Delete(entity);
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion

        #region IDisposable implementation

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
