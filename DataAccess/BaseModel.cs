using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IBaseModel<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll { get; }

        TEntity FindById(object id);

        TEntity Create(TEntity entity);

        TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed);

        TEntity Delete(TEntity entity);

        void SaveChanges();

        JujuTestContext GetContext(); // opcional, puede omitirse si no quieres exponer el contexto
    }

    public class BaseModel<TEntity> : IBaseModel<TEntity> where TEntity : class
    {
        protected readonly JujuTestContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseModel(JujuTestContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll => _dbSet;

        public TEntity FindById(object id) => _dbSet.Find(id);

        public TEntity Create(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed)
        {
            _context.Entry(originalEntity).CurrentValues.SetValues(editedEntity);
            changed = _context.Entry(originalEntity).State == EntityState.Modified;
            _context.SaveChanges();
            return originalEntity;
        }

        public TEntity Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public void SaveChanges() => _context.SaveChanges();

        public JujuTestContext GetContext() => _context;

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }

}
