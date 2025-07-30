using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{

    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly JujuTestContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(JujuTestContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        /// <summary>
        /// Retrieves all entities of type TEntity from the database.
        /// </summary>
        public IQueryable<TEntity> GetAll => _dbSet;

        /// <summary>
        /// Updates an existing entity of type TEntity in the database.
        /// </summary>
        /// <param name="editedEntity"></param>
        /// <param name="originalEntity"></param>
        /// <param name="changed"></param>
        /// <returns></returns>
        public TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed)
        {
            _context.Entry(originalEntity).CurrentValues.SetValues(editedEntity);
            changed = _context.Entry(originalEntity).State == EntityState.Modified;
            _context.SaveChanges();
            return originalEntity;
        }

        /// <summary>
        /// Deletes an entity of type TEntity from the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The added entity</returns>
        public TEntity Create(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }


        /// <summary>
        /// Saves changes made to the context to the database.
        /// </summary>
        public void SaveChanges() => _context.SaveChanges();

        /// <summary>
        /// Asynchronously saves changes made to the context to the database.
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }

}
