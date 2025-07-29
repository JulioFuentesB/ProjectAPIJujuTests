using Business.Common.DTOs.Customer;
using Business.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Common.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<OperationResult<CustomerDto>> CreateAsync(CustomerCreateDto customerDto);
        Task<OperationResult<bool>> DeleteAsync(int id);

        //Task<OperationResult<CustomerDto>> GetCustomerByNameAsync(string name);
        //Task<OperationResult<CustomerDto>> CreateCustomerAsync(CustomerCreateDto customerDto);
        //Task<OperationResult<CustomerDto>> UpdateCustomerAsync(int id, CustomerUpdateDto customerDto);
        //Task<OperationResult<IQueryable<CustomerDto>>> GetAllAsync();
        //Task<OperationResult<CustomerDto>> GetByIdAsync(int id);
        //Task<OperationResult<CustomerDto>> CreateAsync(CustomerCreateDto customerDto);
        //Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerUpdateDto customerDto);
        //Task<OperationResult<bool>> DeleteAsync(int id);
        Task<OperationResult<IEnumerable<CustomerDto>>> GetAllAsync();
        Task<OperationResult<CustomerDto>> GetByIdAsync(int id);
        Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerUpdateDto customerDto);
    }
}
