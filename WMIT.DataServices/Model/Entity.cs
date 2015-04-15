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

        [FieldAccess(IsSystemField=true)]
        public bool IsDeleted { get; set; }

        [FieldAccess(IsSystemField = true)]
        public string CreatedBy { get; set; }

        [FieldAccess(IsSystemField = true)]
        public DateTime CreatedAt { get; set; }

        [FieldAccess(IsSystemField = true)]
        public string ModifiedBy { get; set; }

        [FieldAccess(IsSystemField = true)]
        public DateTime? ModifiedAt { get; set; }
    }

    public interface IEntity
    {
        int Id { get; }

        bool IsDeleted { get; set; }

        string CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }
        string ModifiedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
    }
}
