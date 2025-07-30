using DataAccess.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{


    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        /// <summary>
        /// Retrieves all customers.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Customer>> GetAllAsync();

        /// <summary>
        /// Retrieves a customer by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Customer> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a customer by its ID along with related posts.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Customer> GetByIdWithPostsAsync(int id);

        /// <summary>
        /// Retrieves a customer by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Customer> GetByNameAsync(string name);

        /// <summary>
        /// Adds a new customer to the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(Customer entity);

        /// <summary>
        /// Updates an existing customer in the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(Customer entity);

        /// <summary>
        /// Deletes a customer from the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task RemoveAsync(Customer entity);

        /// <summary>
        /// Deletes a range of customers from the database.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task RemoveRangeAsync(IEnumerable<Post> entities);


    }
}
