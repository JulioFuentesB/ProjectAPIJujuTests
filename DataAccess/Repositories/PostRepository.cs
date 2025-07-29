using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{


    public class PostRepository : BaseModel<Post>, IPostRepository
    {
        public PostRepository(JujuTestContext context) : base(context)
        {
        }

        public new IQueryable<Post> GetAll() => base.GetAll.Include(p => p.Customer);

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PostId == id);
        }

        // Los métodos Create, Update, Remove ya están heredados de BaseModel
        // y cumplen con la interfaz IPostRepository

        public void RemoveRange(IEnumerable<Post> entities)
        {
            _dbSet.RemoveRange(entities);
            SaveChanges();
        }

        public IQueryable<Post> GetByCustomerId(int customerId) =>
            GetAll().Where(p => p.CustomerId == customerId);

        void IPostRepository.Create(Post entity)
        {
            _dbSet.Add(entity);
            SaveChanges();
        }

        public void AddRange(IEnumerable<Post> entities)
        {
            _dbSet.AddRange(entities);
            SaveChanges();
        }


        public async Task UpdateAsync(Post entity)
        {
            var original = await GetByIdAsync(entity.PostId);
            if (original != null)
            {
                Update(entity, original, out _);
            }
        }

        public void Remove(Post entity)
        {
            Delete(entity); // Utilizamos el método Delete heredado de BaseModel
        }

        public async Task AddRangeAsync(IEnumerable<Post> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await SaveChangesAsync(); // Asume que tienes este método en BaseModel
        }



    }
}
