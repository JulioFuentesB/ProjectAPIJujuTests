using Business.Common.DTOs.Customer;
using Business.Common.Interfaces.Services;
using Business.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService,
                                  ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                OperationResult<IEnumerable<CustomerDto>> result = await _customerService.GetAllAsync();
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                OperationResult<CustomerDto> result = await _customerService.GetByIdAsync(id);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving customer with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto customerDto)
        {
            try
            {
                OperationResult<CustomerDto> result = await _customerService.CreateAsync(customerDto);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return CreatedAtAction(nameof(GetById), new { id = result.Data.CustomerId }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto customerDto)
        {
            try
            {
                OperationResult<CustomerDto> result = await _customerService.UpdateAsync(id, customerDto);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating customer with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                OperationResult<bool> result = await _customerService.DeleteAsync(id);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
