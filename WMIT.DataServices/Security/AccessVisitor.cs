﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WMIT.DataServices.Common;

namespace WMIT.DataServices.Security
{
    public class AccessVisitor : IEntityVisitor
    {
        public virtual void Visit(EntityContext context)
        {
            // Test entity for validity
            var entityType = context.Entry.Entity.GetType();
            var entityAttributes = entityType.GetCustomAttributes<AccessAttribute>(true);

            foreach (var attr in entityAttributes)
            {
                if (attr.On != EntityOperation.All && !attr.On.HasFlag(context.Operation))
                    continue;

                var valid = this.PrincipalHasAccess(context.User, attr);

                if (!valid)
                    HandleViolation(attr, context);
            }

            // Test modified properties for validity
            var properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var modifiedProperties = properties.Where(p => context.Entry.Member(p.Name) is DbPropertyEntry
                && (context.Entry.Property(p.Name).IsModified || context.Entry.State == EntityState.Added)).ToList();

            foreach (var property in modifiedProperties)
            {
                var propertyAttributes = property.GetCustomAttributes<AccessAttribute>(false);

                foreach (var attr in propertyAttributes)
                {
                    if (attr.On != EntityOperation.All && !attr.On.HasFlag(context.Operation))
                        continue;

                    var valid = this.PrincipalHasAccess(context.User, attr);

                    if (!valid)
                        HandleViolation(attr, context, property);
                }
            }
        }

        public virtual bool PrincipalHasAccess(IPrincipal principal, AccessAttribute attr)
        {
            if (attr.InternalUsage || principal == null || !principal.Identity.IsAuthenticated)
                return false;

            if (attr.Users != null)
            {
                var users = attr.Users.ToLower().Split(new char[] { ',' });
                if (!users.Contains(principal.Identity.Name.ToLower()))
                {
                    return false;
                }
            }

            if (attr.Roles != null)
            {
                var roles = attr.Roles.ToLower().Split(new char[] { ',' });

                if (!roles.Any(role => principal.IsInRole(role)))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void HandleViolation(AccessAttribute attr, EntityContext context, PropertyInfo property = null)
        {
            if (attr.ViolationBehavior == ViolationBehavior.IgnoreUserInput)
            {
                // Ignore changes in violation, set states to unchanged/not modified

                if (property == null)
                {
                    context.Entry.State = EntityState.Unchanged;
                }
                else
                {
                    context.Entry.Property(property.Name).IsModified = false;
                }
            }
            else
            {
                throw new DataServicesAccessViolationException();
            }
        }

    }
}
