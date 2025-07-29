using DataAccess.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll { get; }

        TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed);

        TEntity Delete(TEntity entity);

        void SaveChanges();
        Task SaveChangesAsync();
    }

}
