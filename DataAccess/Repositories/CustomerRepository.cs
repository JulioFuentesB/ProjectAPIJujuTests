using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{

    public class CustomerRepository : BaseModel<Customer>, ICustomerRepository
    {
        public CustomerRepository(JujuTestContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync();
        }

        public async Task<Customer> GetByIdWithPostsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<Customer> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task AddAsync(Customer entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer entity)
        {
            var original = await GetByIdAsync(entity.CustomerId);
            if (original != null)
            {
                _context.Entry(original).CurrentValues.SetValues(entity);
                await SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(Customer entity)
        {
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Post> entities)
        {
            _context.Set<Post>().RemoveRange(entities);
            await SaveChangesAsync();
        }

        public new async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}