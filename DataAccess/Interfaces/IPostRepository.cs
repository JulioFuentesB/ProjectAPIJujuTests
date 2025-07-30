using DataAccess.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IPostRepository : IBaseRepository<Post>
    {

        /// <summary>
        /// Retrieves a post by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Post> GetByIdAsync(int id);

        /// <summary>
        /// Updates an existing post in the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(Post entity);

        /// <summary>
        /// Deletes a range of posts from the database.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<Post> entities);
    }
}
