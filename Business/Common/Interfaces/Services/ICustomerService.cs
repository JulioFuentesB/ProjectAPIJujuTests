using Business.Common.DTOs.Customer;
using Business.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Common.Interfaces.Services
{
    public interface ICustomerService
    {
        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de clientes</returns>
        Task<OperationResult<IEnumerable<CustomerDto>>> GetAllAsync();

        /// <summary>
        /// Obtiene un cliente por su ID.
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Resultado de la operación con el cliente encontrado</returns>
        Task<OperationResult<CustomerDto>> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo cliente.
        /// </summary>
        /// <param name="customerDto">Datos del nuevo cliente</param>
        /// <returns>Resultado de la operación con el cliente creado</returns>
        Task<OperationResult<CustomerDto>> CreateAsync(CustomerCreateDto customerDto);

        /// <summary>
        /// Actualiza un cliente existente.
        /// </summary>
        /// <param name="id">ID del cliente a actualizar</param>
        /// <param name="customerDto">Datos actualizados del cliente</param>
        /// <returns>Resultado de la operación con el cliente actualizado</returns>
        Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerUpdateDto customerDto);

        /// <summary>
        /// Elimina un cliente por su ID, incluyendo los posts asociados si existen.
        /// </summary>
        /// <param name="id">ID del cliente a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<OperationResult<bool>> DeleteAsync(int id);
    }

}
