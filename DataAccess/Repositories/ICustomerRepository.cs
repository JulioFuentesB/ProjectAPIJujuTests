using DataAccess.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{

    /// public interface INpedidoTempRepository : IGenericRepository<NpedidoTemp>
    public interface ICustomerRepository : IBaseModel<Customer>
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> GetByIdWithPostsAsync(int id);
        Task<Customer> GetByNameAsync(string name);
        Task AddAsync(Customer entity);
        Task UpdateAsync(Customer entity);
        Task RemoveAsync(Customer entity);
        Task RemoveRangeAsync(IEnumerable<Post> entities);
        Task SaveChangesAsync();
    }
}
