using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Common;

namespace WMIT.DataServices.Visitors
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoValueAttribute : Attribute
    {
        public Type Strategy { get; set; }
        public EntityOperation On { get; set; }
    }

    public interface IAutoValueStrategy
    {
        object GetValue(EntityContext context);
    }

    public class AutoValueVisitor : IEntityVisitor
    {
        public void Visit(EntityContext context)
        {
            var properties = context.Entry.Entity.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<AutoValueAttribute>(false) != null);

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<AutoValueAttribute>();
                if (attr.On != EntityOperation.All && !attr.On.HasFlag(context.Operation))
                    continue;

                var strategy = (IAutoValueStrategy)Activator.CreateInstance(attr.Strategy);
                var value = strategy.GetValue(context);

                property.SetValue(context.Entry.Entity, value);

                if (context.Entry.State == EntityState.Modified || context.Entry.State == EntityState.Unchanged)
                    context.Entry.Property(property.Name).IsModified = true;
            }
        }
    }
}
