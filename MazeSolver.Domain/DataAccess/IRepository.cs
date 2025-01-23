using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver.Domain.DataAccess
{
    public interface IRepository<TEntity, TKey>
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TKey id);
        TEntity Get(TKey id);
        IEnumerable<TEntity> GetAll();
    }
}
