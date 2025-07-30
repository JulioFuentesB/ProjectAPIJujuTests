using AutoMapper;
using Business.Common.Interfaces.Services;
using Business.Results;
using DataAccess.Data;
using DataAccess.Interfaces;
using global::Business.Common.DTOs.Customer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(
            IPostRepository postRepository,
            IMapper mapper,
            ICustomerRepository customerRepository)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de clientes</returns>
        public async Task<OperationResult<IEnumerable<CustomerDto>>> GetAllAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var result = _mapper.Map<IEnumerable<CustomerDto>>(customers);
                return OperationResult<IEnumerable<CustomerDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<CustomerDto>>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene un cliente por su ID.
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Resultado de la operación con el cliente encontrado</returns>
        public async Task<OperationResult<CustomerDto>> GetByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    return OperationResult<CustomerDto>.NotFound($"No se encontró el cliente con ID {id}.");
                }

                var result = _mapper.Map<CustomerDto>(customer);
                return OperationResult<CustomerDto>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<CustomerDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Crea un nuevo cliente.
        /// </summary>
        /// <param name="customerDto">Datos del nuevo cliente</param>
        /// <returns>Resultado de la operación con el cliente creado</returns>
        public async Task<OperationResult<CustomerDto>> CreateAsync(CustomerCreateDto customerDto)
        {
            try
            {
                // Validar nombre único
                var existingCustomer = await _customerRepository.GetByNameAsync(customerDto.Name);

                if (existingCustomer != null)
                {
                    return OperationResult<CustomerDto>.Conflict($"Ya existe un cliente con el nombre '{customerDto.Name}'.");
                }

                var customer = _mapper.Map<Customer>(customerDto);
                await _customerRepository.AddAsync(customer);

                var createdCustomer = _mapper.Map<CustomerDto>(customer);
                return OperationResult<CustomerDto>.Ok(createdCustomer);
            }
            catch (Exception ex)
            {
                return OperationResult<CustomerDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Actualiza un cliente existente.
        /// </summary>
        /// <param name="id">ID del cliente a actualizar</param>
        /// <param name="customerDto">Datos actualizados del cliente</param>
        /// <returns>Resultado de la operación con el cliente actualizado</returns>
        public async Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerUpdateDto customerDto)
        {
            try
            {
                var originalCustomer = await _customerRepository.GetByIdAsync(id);

                if (originalCustomer == null)
                {
                    return OperationResult<CustomerDto>.NotFound($"No se encontró el cliente con ID {id}.");
                }

                _mapper.Map(customerDto, originalCustomer);
                await _customerRepository.UpdateAsync(originalCustomer);

                var updatedCustomer = _mapper.Map<CustomerDto>(originalCustomer);
                return OperationResult<CustomerDto>.Ok(updatedCustomer);
            }
            catch (Exception ex)
            {
                return OperationResult<CustomerDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Elimina un cliente por su ID, incluyendo los posts asociados si existen.
        /// </summary>
        /// <param name="id">ID del cliente a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdWithPostsAsync(id);

                if (customer == null)
                {
                    return OperationResult<bool>.NotFound($"No se encontró el cliente con ID {id}.");
                }

                // Eliminar posts asociados si existen
                if (customer.Posts != null && customer.Posts.Any())
                {
                    await _customerRepository.RemoveRangeAsync(customer.Posts);
                }

                await _customerRepository.RemoveAsync(customer);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }
}


