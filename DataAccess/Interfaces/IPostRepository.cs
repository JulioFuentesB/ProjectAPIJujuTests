using DataAccess.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IPostRepository : IBaseRepository<Post>
    {
        new IQueryable<Post> GetAll();
        Task<Post> GetByIdAsync(int id);
        new void Create(Post entity);
        Task UpdateAsync(Post entity);
        void Remove(Post entity);
        Task AddRangeAsync(IEnumerable<Post> entities);
    }
}
