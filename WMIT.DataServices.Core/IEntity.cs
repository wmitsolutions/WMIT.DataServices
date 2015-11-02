using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Core
{
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
