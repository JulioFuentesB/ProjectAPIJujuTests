using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        IQueryable<TEntity> GetAll { get; }

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="editedEntity"></param>
        /// <param name="originalEntity"></param>
        /// <param name="changed"></param>
        /// <returns></returns>
        TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed);

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Remove(TEntity entity);

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Asynchronously saves changes to the database.
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
        TEntity Create(TEntity entity);
    }

}
