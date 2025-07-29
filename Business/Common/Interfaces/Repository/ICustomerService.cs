using Business.Common.DTOs.Customer;
using Business.Results;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Common.Interfaces.Repository
{
    public interface ICustomerService
    {
        Task<OperationResult<IQueryable<CustomerDto>>> GetAllAsync();
        Task<OperationResult<CustomerDto>> GetByIdAsync(int id);
        Task<OperationResult<CustomerDto>> CreateAsync(CustomerCreateDto customerDto);
        Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerUpdateDto customerDto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }
}

