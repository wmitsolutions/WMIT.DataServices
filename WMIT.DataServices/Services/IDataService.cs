using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Services
{
    public interface IDataService<T>
    {
        IQueryable<T> Entities { get; }

        Task Insert(T item);
        Task Update(T item);
        Task Delete(T item);
    }
}
