using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Security;

namespace WMIT.DataServices.Model
{
    public class Entity : IEntity
    {
        public int Id { get; set; }

        [Access(InternalUsage = true, ViolationBehavior = ViolationBehavior.Throw)]
        public bool IsDeleted { get; set; }

        [Access(InternalUsage = true, ViolationBehavior = ViolationBehavior.Throw)]
        public string CreatedBy { get; set; }

        [Access(InternalUsage = true, ViolationBehavior = ViolationBehavior.Throw)]
        public DateTimeOffset CreatedAt { get; set; }

        [Access(InternalUsage = true, ViolationBehavior = ViolationBehavior.Throw)]
        public string ModifiedBy { get; set; }

        [Access(InternalUsage = true, ViolationBehavior = ViolationBehavior.Throw)]
        public DateTimeOffset? ModifiedAt { get; set; }
    }

    public interface IEntity
    {
        int Id { get; }

        bool IsDeleted { get; set; }

        string CreatedBy { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        string ModifiedBy { get; set; }
        DateTimeOffset? ModifiedAt { get; set; }
    }
}
