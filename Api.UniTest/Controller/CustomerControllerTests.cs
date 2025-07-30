using API.Controllers;
using Business.Common.DTOs.Customer;
using Business.Common.Interfaces.Services;
using Business.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Api.UniTest.Controller
{

    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<ILogger<CustomerController>> _mockLogger;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockLogger = new Mock<ILogger<CustomerController>>();
            _controller = new CustomerController(_mockCustomerService.Object, _mockLogger.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfCustomers()
        {
            // Arrange
            var customers = new List<CustomerDto> { new CustomerDto(), new CustomerDto() };
            _mockCustomerService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new OperationResult<IEnumerable<CustomerDto>>
                {
                    Success = true,
                    Data = customers
                });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customers, okResult.Value);
        }

        [Fact]
        public async Task GetAll_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockCustomerService.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            // Verify logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error retrieving all customers")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenCustomerExists()
        {
            // Arrange
            var customer = new CustomerDto { CustomerId = 1 };
            _mockCustomerService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = true,
                    Data = customer
                });

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customer, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _mockCustomerService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Customer not found"
                });

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(new { message = "Customer not found" }.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetById_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockCustomerService.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error retrieving customer with ID 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ReturnsCreatedResult_WhenCustomerIsCreated()
        {
            // Arrange
            var customerDto = new CustomerCreateDto { Name = "Test" };
            var createdCustomer = new CustomerDto { CustomerId = 1, Name = "Test" };

            _mockCustomerService.Setup(x => x.CreateAsync(customerDto))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = createdCustomer
                });

            // Act
            var result = await _controller.Create(customerDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CustomerController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(createdCustomer, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenInputIsInvalid()
        {
            // Arrange
            var customerDto = new CustomerCreateDto();
            _mockCustomerService.Setup(x => x.CreateAsync(customerDto))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Name is required"
                });

            // Act
            var result = await _controller.Create(customerDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(new { message = "Name is required" }.ToString(), badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenCustomerAlreadyExists()
        {
            // Arrange
            var customerDto = new CustomerCreateDto { Name = "Existing" };
            _mockCustomerService.Setup(x => x.CreateAsync(customerDto))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = false,
                    StatusCode = 409,
                    ErrorMessage = "Customer already exists"
                });

            // Act
            var result = await _controller.Create(customerDto);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, conflictResult.StatusCode);
            Assert.Equal(new { message = "Customer already exists" }.ToString(), conflictResult.Value.ToString());
        }

        [Fact]
        public async Task Create_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var customerDto = new CustomerCreateDto();
            _mockCustomerService.Setup(x => x.CreateAsync(customerDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Create(customerDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error creating customer")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var customerDto = new CustomerUpdateDto { Name = "Updated" };
            var updatedCustomer = new CustomerDto { CustomerId = 1, Name = "Updated" };

            _mockCustomerService.Setup(x => x.UpdateAsync(1, customerDto))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = true,
                    Data = updatedCustomer
                });

            // Act
            var result = await _controller.Update(1, customerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedCustomer, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenInputIsInvalid()
        {
            // Arrange
            var customerDto = new CustomerUpdateDto();
            _mockCustomerService.Setup(x => x.UpdateAsync(1, customerDto))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Name is required"
                });

            // Act
            var result = await _controller.Update(1, customerDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(new { message = "Name is required" }.ToString(), badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerDto = new CustomerUpdateDto { Name = "Test" };
            _mockCustomerService.Setup(x => x.UpdateAsync(1, customerDto))
                .ReturnsAsync(new OperationResult<CustomerDto>
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Customer not found"
                });

            // Act
            var result = await _controller.Update(1, customerDto);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(new { message = "Customer not found" }.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task Update_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var customerDto = new CustomerUpdateDto();
            _mockCustomerService.Setup(x => x.UpdateAsync(1, customerDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Update(1, customerDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error updating customer with ID 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            _mockCustomerService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(new OperationResult<bool>
                {
                    Success = true
                });

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _mockCustomerService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(new OperationResult<bool>
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Customer not found"
                });

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(new { message = "Customer not found" }.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task Delete_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockCustomerService.Setup(x => x.DeleteAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error deleting customer with ID 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion
    }

}
