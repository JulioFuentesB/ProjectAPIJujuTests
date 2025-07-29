using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{

    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
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
            return await _dbSet.FirstOrDefaultAsync(x => x.CustomerId == id);
        }

        public async Task<Customer> GetByIdWithPostsAsync(int id)
        {
            // Pseudocódigo:
            // 1. Buscar el cliente por id.
            // 2. Si existe, obtener sus posts mediante una subconsulta.
            // 3. Asignar los posts al cliente y devolverlo.

            var customer = await _dbSet.FirstOrDefaultAsync(c => c.CustomerId == id);

            // Si el cliente no existe, retornar null
            if (customer != null)
            {
                customer.Posts = await _context.Set<Post>()
                    .Where(p => p.CustomerId == id)
                    .ToListAsync();
            }
            return customer;
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