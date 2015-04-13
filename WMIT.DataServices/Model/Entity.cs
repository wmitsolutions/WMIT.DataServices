using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Model
{
    public class Entity : IEntity
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
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
