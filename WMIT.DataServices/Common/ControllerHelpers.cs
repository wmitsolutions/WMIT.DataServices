using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Model;
using WMIT.DataServices.Security;

namespace WMIT.DataServices.Common
{
    public static class ControllerHelpers
    {
        public static DbEntityEntry<T> Update<T>(this DbEntityEntry<T> entry) where T : class, IEntity
        {
            entry.State = EntityState.Modified;

            foreach (var property in entry.Entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var fieldAccess = property.GetCustomAttribute<FieldAccessAttribute>();

                if (fieldAccess == null)
                    continue;

                // If the field is system exclusive (IsDeleted, CreatedBy, ModifiedAt...)
                // unset modification to get no update if the user passed a value
                if (fieldAccess.IsSystemField)
                {
                    entry.Property(property.Name).IsModified = false;
                }
            }

            return entry;
        }

        public static void SetModificationStatistics<T>(this DbEntityEntry<T> entry, IIdentity user) where T : class, IEntity
        {
            entry.Entity.ModifiedBy = user.Name;
            entry.Property(p => p.ModifiedBy).IsModified = true;

            entry.Entity.ModifiedAt = DateTime.Now;
            entry.Property(p => p.ModifiedAt).IsModified = true;
        }

        public static void SetCreationStatistics<T>(this T entity, IIdentity user) where T : class, IEntity
        {
            entity.CreatedBy = user.Name;
            entity.CreatedAt = DateTime.Now;

            entity.ModifiedBy = null;
            entity.ModifiedAt = null;
            entity.IsDeleted = false;
        }
    }
}
