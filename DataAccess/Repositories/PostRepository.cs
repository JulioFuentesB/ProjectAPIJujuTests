using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{


    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(JujuTestContext context) : base(context)
        {
        }

        public new IQueryable<Post> GetAll()
        {
            // Devuelve los posts sin incluir el Customer (consulta simple)
            return base.GetAll;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            // Obtiene el post sin incluir el Customer
            var post = await _dbSet.FirstOrDefaultAsync(p => p.PostId == id);
            if (post != null)
            {
                // Realiza una segunda consulta para obtener el Customer
                post.Customer = await _context.Customer.FirstOrDefaultAsync(c => c.CustomerId == post.CustomerId);
            }
            return post;
        }


        void IPostRepository.Create(Post entity)
        {
            _dbSet.Add(entity);
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
