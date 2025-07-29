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

        /// <summary>
        /// Retrieves all posts from the database without including the Customer entity.
        /// </summary>
        /// <returns></returns>
        public new IQueryable<Post> GetAll()
        {
            // Devuelve los posts sin incluir el Customer (consulta simple)
            return base.GetAll;
        }

        /// <summary>
        /// Retrieves all posts from the database, including the Customer entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a new post in the database.
        /// </summary>
        /// <param name="entity"></param>
        void IPostRepository.Create(Post entity)
        {
            _dbSet.Add(entity);
            SaveChanges();
        }

        /// <summary>
        /// Updates an existing post in the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Post entity)
        {
            var original = await GetByIdAsync(entity.PostId);
            if (original != null)
            {
                Update(entity, original, out _);
            }
        }

        /// <summary>
        /// Deletes a post from the database.
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(Post entity)
        {
            Delete(entity); // Utilizamos el método Delete heredado de BaseModel
        }

        /// <summary>
        /// Adds a new post to the database asynchronously.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task AddRangeAsync(IEnumerable<Post> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await SaveChangesAsync(); // Asume que tienes este método en BaseModel
        }



    }
}
