using DataAccess.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IPostRepository : IBaseModel<Post>
    {
        IQueryable<Post> GetAll();
        Task<Post> GetByIdAsync(int id);
        void Create(Post entity);
        Task UpdateAsync(Post entity);
        void Remove(Post entity);
        void RemoveRange(IEnumerable<Post> entities);
        IQueryable<Post> GetByCustomerId(int customerId);
        void AddRange(IEnumerable<Post> entities);
        Task AddRangeAsync(IEnumerable<Post> entities);
    }
}
