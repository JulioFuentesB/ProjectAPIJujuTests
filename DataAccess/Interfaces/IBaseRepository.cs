using DataAccess.Data;
using System.Linq;

namespace DataAccess.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll { get; }

        TEntity FindById(object id);

        TEntity Create(TEntity entity);

        TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed);

        TEntity Delete(TEntity entity);

        void SaveChanges();

        JujuTestContext GetContext(); // opcional, puede omitirse si no quieres exponer el contexto
    }

}
