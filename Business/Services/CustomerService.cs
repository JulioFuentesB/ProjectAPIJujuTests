using AutoMapper;
using Business.Results;
using DataAccess.Data;
using DataAccess.Repositories;
using global::Business.Common.DTOs.Customer;
using global::Business.Common.Interfaces.Services;
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

        public async Task<OperationResult<CustomerDto>> GetByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return OperationResult<CustomerDto>.NotFound($"Customer with ID {id} not found.");
                }

                var result = _mapper.Map<CustomerDto>(customer);
                return OperationResult<CustomerDto>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<CustomerDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<OperationResult<CustomerDto>> CreateAsync(CustomerCreateDto customerDto)
        {
            try
            {
                // Validar nombre único
                var existingCustomer = await _customerRepository.GetByNameAsync(customerDto.Name);
                if (existingCustomer != null)
                {
                    return OperationResult<CustomerDto>.Conflict($"A customer with the name '{customerDto.Name}' already exists.");
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

        public async Task<OperationResult<CustomerDto>> UpdateAsync(int id, CustomerUpdateDto customerDto)
        {
            try
            {
                var originalCustomer = await _customerRepository.GetByIdAsync(id);
                if (originalCustomer == null)
                {
                    return OperationResult<CustomerDto>.NotFound($"Customer with ID {id} not found.");
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

        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdWithPostsAsync(id);
                if (customer == null)
                {
                    return OperationResult<bool>.NotFound($"Customer with ID {id} not found.");
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


