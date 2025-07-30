using AutoMapper;
using Business.Common.DTOs.Customer;
using Business.Services;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace Business.UniTest.Sevices
{

    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockPostRepository = new Mock<IPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _customerService = new CustomerService(
                _mockPostRepository.Object,
                _mockMapper.Object,
                _mockCustomerRepository.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsSuccessWithCustomers_WhenCustomersExist()
        {
            // Arrange
            var customers = new List<Customer> { new Customer(), new Customer() };
            var customerDtos = new List<CustomerDto> { new CustomerDto(), new CustomerDto() };

            _mockCustomerRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(customers);
            _mockMapper.Setup(x => x.Map<IEnumerable<CustomerDto>>(customers))
                .Returns(customerDtos);

            // Act
            var result = await _customerService.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(customerDtos, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoCustomersExist()
        {
            // Arrange
            var customers = new List<Customer>();
            var customerDtos = new List<CustomerDto>();

            _mockCustomerRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(customers);
            _mockMapper.Setup(x => x.Map<IEnumerable<CustomerDto>>(customers))
                .Returns(customerDtos);

            // Act
            var result = await _customerService.GetAllAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.GetAllAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsSuccessWithCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1 };
            var customerDto = new CustomerDto { CustomerId = 1 };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(customer);
            _mockMapper.Setup(x => x.Map<CustomerDto>(customer))
                .Returns(customerDto);

            // Act
            var result = await _customerService.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(customerDto, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetByIdAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.GetByIdAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsSuccessWithCustomer_WhenCreationIsSuccessful()
        {
            // Arrange
            var customerDto = new CustomerCreateDto { Name = "Test" };
            var customer = new Customer { CustomerId = 1, Name = "Test" };
            var createdCustomerDto = new CustomerDto { CustomerId = 1, Name = "Test" };

            _mockCustomerRepository.Setup(x => x.GetByNameAsync("Test"))
                .ReturnsAsync((Customer)null);
            _mockMapper.Setup(x => x.Map<Customer>(customerDto))
                .Returns(customer);
            _mockMapper.Setup(x => x.Map<CustomerDto>(customer))
                .Returns(createdCustomerDto);

            // Act
            var result = await _customerService.CreateAsync(customerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(createdCustomerDto, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            _mockCustomerRepository.Verify(x => x.AddAsync(customer), Moq.Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ReturnsConflict_WhenCustomerNameExists()
        {
            // Arrange
            var customerDto = new CustomerCreateDto { Name = "Existing" };
            var existingCustomer = new Customer { CustomerId = 1, Name = "Existing" };

            _mockCustomerRepository.Setup(x => x.GetByNameAsync("Existing"))
                .ReturnsAsync(existingCustomer);

            // Act
            var result = await _customerService.CreateAsync(customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        }

        [Fact]
        public async Task CreateAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            var customerDto = new CustomerCreateDto { Name = "Test" };

            _mockCustomerRepository.Setup(x => x.GetByNameAsync("Test"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.CreateAsync(customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsSuccessWithCustomer_WhenUpdateIsSuccessful()
        {
            // Arrange
            var customerDto = new CustomerUpdateDto { Name = "Updated" };
            var originalCustomer = new Customer { CustomerId = 1, Name = "Original" };
            var updatedCustomerDto = new CustomerDto { CustomerId = 1, Name = "Updated" };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(originalCustomer);
            _mockMapper.Setup(x => x.Map<CustomerDto>(originalCustomer))
                .Returns(updatedCustomerDto);

            // Act
            var result = await _customerService.UpdateAsync(1, customerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(updatedCustomerDto, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            _mockCustomerRepository.Verify(x => x.UpdateAsync(originalCustomer), Moq.Times.Once);
            _mockMapper.Verify(x => x.Map(customerDto, originalCustomer), Moq.Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerDto = new CustomerUpdateDto { Name = "Test" };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.UpdateAsync(1, customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            var customerDto = new CustomerUpdateDto { Name = "Test" };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.UpdateAsync(1, customerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ReturnsSuccess_WhenDeletionIsSuccessful()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1 };
            customer.Posts = new List<Post>();

            _mockCustomerRepository.Setup(x => x.GetByIdWithPostsAsync(1))
                .ReturnsAsync(customer);

            // Act
            var result = await _customerService.DeleteAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            _mockCustomerRepository.Verify(x => x.RemoveAsync(customer), Moq.Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletesPosts_WhenCustomerHasPosts()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1 };
            customer.Posts = new List<Post> { new Post(), new Post() };

            _mockCustomerRepository.Setup(x => x.GetByIdWithPostsAsync(1))
                .ReturnsAsync(customer);

            // Act
            var result = await _customerService.DeleteAsync(1);

            // Assert
            Assert.True(result.Success);
            _mockCustomerRepository.Verify(x => x.RemoveRangeAsync(customer.Posts), Moq.Times.Once);
            _mockCustomerRepository.Verify(x => x.RemoveAsync(customer), Moq.Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetByIdWithPostsAsync(1))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.DeleteAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetByIdWithPostsAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _customerService.DeleteAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion
    }
}




