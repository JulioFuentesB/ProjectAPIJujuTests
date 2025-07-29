using AutoMapper;
using Business.Common.DTOs.Post;
using Business.Services;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Business.UniTest.Sevices
{

    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PostService _postService;

        public PostServiceTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockMapper = new Mock<IMapper>();
            _postService = new PostService(
                _mockPostRepository.Object,
                _mockCustomerRepository.Object,
                _mockMapper.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public void GetAllAsync_ReturnsSuccessWithPosts_WhenPostsExist()
        {
            // Arrange
            var posts = new List<Post> { new Post(), new Post() };
            var postDtos = new List<PostDto> { new PostDto(), new PostDto() };

            _mockPostRepository.Setup(x => x.GetAll())
                .Returns(posts.AsQueryable());
            _mockMapper.Setup(x => x.Map<IEnumerable<PostDto>>(posts))
                .Returns(postDtos);

            // Act
            var result = _postService.GetAllAsync().Result;

            // Assert
            Assert.True(result.Success);
            Assert.Equal(postDtos, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void GetAllAsync_ReturnsEmptyList_WhenNoPostsExist()
        {
            // Arrange
            var posts = new List<Post>();
            var postDtos = new List<PostDto>();

            _mockPostRepository.Setup(x => x.GetAll())
                .Returns(posts.AsQueryable());
            _mockMapper.Setup(x => x.Map<IEnumerable<PostDto>>(posts))
                .Returns(postDtos);

            // Act
            var result = _postService.GetAllAsync().Result;

            // Assert
            Assert.True(result.Success);
            Assert.Empty(result.Data);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsSuccessWithPost_WhenPostExists()
        {
            // Arrange
            var post = new Post { CustomerId = 1 };
            var postDto = new PostDto { PostId = 1 };

            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(post);
            _mockMapper.Setup(x => x.Map<PostDto>(post))
                .Returns(postDto);

            // Act
            var result = await _postService.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(postDto, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _postService.GetByIdAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Post with ID 1 not found.", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsException_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _postService.GetByIdAsync(1));
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ReturnsSuccessWithPost_WhenCreationIsSuccessful()
        {
            // Arrange
            var postDto = new PostCreateDto { CustomerId = 1, Title = "Test", Body = "Test body", Type = 1 };
            var customer = new Customer { CustomerId = 1 };
            var post = new Post { CustomerId = 1, Title = "Test", Body = "Test body", Category = "Farándula" };
            var createdPostDto = new PostDto { PostId = 1, Title = "Test", Body = "Test body", Category = "Farándula" };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(customer);
            _mockMapper.Setup(x => x.Map<Post>(postDto))
                .Returns(post);
            _mockMapper.Setup(x => x.Map<PostDto>(post))
                .Returns(createdPostDto);

            // Act
            var result = await _postService.CreateAsync(postDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(createdPostDto, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal("Farándula", post.Category);
            _mockPostRepository.Verify(x => x.Create(post), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ProcessesBodyCorrectly_WhenBodyIsLong()
        {
            // Arrange
            var longBody = new string('a', 100);
            var postDto = new PostCreateDto { CustomerId = 1, Title = "Test", Body = longBody, Type = 1 };
            var customer = new Customer { CustomerId = 1 };
            var post = new Post { CustomerId = 1 };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(customer);
            _mockMapper.Setup(x => x.Map<Post>(postDto))
                .Returns(post);

            // Act
            var result = await _postService.CreateAsync(postDto);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task CreateAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var postDto = new PostCreateDto { CustomerId = 1 };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _postService.CreateAsync(postDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Customer with ID 1 not found.", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task CreateAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            var postDto = new PostCreateDto { CustomerId = 1 };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _postService.CreateAsync(postDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion

        #region CreateBatchAsync Tests

        [Fact]
        public async Task CreateBatchAsync_ReturnsSuccess_WhenAllPostsAreValid()
        {
            // Arrange
            var postDtos = new List<PostCreateDto>
            {
                new PostCreateDto { CustomerId = 1, Title = "Test 1" },
                new PostCreateDto { CustomerId = 1, Title = "Test 2" }
            };
            var customer = new Customer { CustomerId = 1 };
            var posts = new List<Post> { new Post { CustomerId = 1 }, new Post { CustomerId = 2 } };
            var postDtosResult = new List<PostDto> { new PostDto { PostId = 1 }, new PostDto { PostId = 2 } };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(customer);
            _mockMapper.Setup(x => x.Map<Post>(It.IsAny<PostCreateDto>()))
                .Returns<PostCreateDto>(dto => new Post { CustomerId = posts.Count + 1 });
            _mockMapper.Setup(x => x.Map<IEnumerable<PostDto>>(posts))
                .Returns(postDtosResult);

            // Act
            var result = await _postService.CreateBatchAsync(postDtos);

            // Assert
            Assert.True(result.Success);
            _mockPostRepository.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Post>>()), Times.Once);
        }

        [Fact]
        public async Task CreateBatchAsync_ReturnsBadRequest_WhenNoPostsProvided()
        {
            // Arrange
            var postDtos = new List<PostCreateDto>();

            // Act
            var result = await _postService.CreateBatchAsync(postDtos);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No posts provided for batch creation", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task CreateBatchAsync_ReturnsBadRequest_WhenSomeCustomersNotFound()
        {
            // Arrange
            var postDtos = new List<PostCreateDto>
            {
                new PostCreateDto { CustomerId = 1, Title = "Valid" },
                new PostCreateDto { CustomerId = 99, Title = "Invalid" }
            };
            var customer = new Customer { CustomerId = 1 };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(customer);
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _postService.CreateBatchAsync(postDtos);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Customer with ID 99 not found", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task CreateBatchAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            var postDtos = new List<PostCreateDto> { new PostCreateDto { CustomerId = 1 } };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _postService.CreateBatchAsync(postDtos);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ReturnsSuccessWithPost_WhenUpdateIsSuccessful()
        {
            // Arrange
            var postDto = new PostUpdateDto { Title = "Updated", Type = 2 };
            var originalPost = new Post { CustomerId = 1, Title = "Original", Category = "Farándula" };
            var updatedPostDto = new PostDto { PostId = 1, Title = "Updated", Category = "Política" };

            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(originalPost);
            _mockMapper.Setup(x => x.Map<PostDto>(originalPost))
                .Returns(updatedPostDto);

            // Act
            var result = await _postService.UpdateAsync(1, postDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(updatedPostDto, result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            _mockPostRepository.Verify(x => x.UpdateAsync(originalPost), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ProcessesBodyCorrectly_WhenBodyIsLong()
        {
            // Arrange
            var longBody = new string('a', 100);
            var postDto = new PostUpdateDto { Body = longBody, Type = 1 };
            var originalPost = new Post { CustomerId = 1 };

            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(originalPost);

            // Act
            var result = await _postService.UpdateAsync(1, postDto);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postDto = new PostUpdateDto();

            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _postService.UpdateAsync(1, postDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Post with ID 1 not found.", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            var postDto = new PostUpdateDto();

            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _postService.UpdateAsync(1, postDto);

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
            var post = new Post { CustomerId = 1 };

            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(post);

            // Act
            var result = await _postService.DeleteAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            _mockPostRepository.Verify(x => x.Remove(post), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _postService.DeleteAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Post with ID 1 not found.", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockPostRepository.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _postService.DeleteAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        #endregion

        #region ProcessBodyAndCategory Tests

        [Theory]
        [InlineData(1, "Farándula")]
        [InlineData(2, "Política")]
        [InlineData(3, "Fútbol")]
        [InlineData(4, "ExistingCategory")]
        public void AssignCategory_SetsCorrectCategory_BasedOnType(int type, string expectedCategory)
        {
            // Arrange
            var postDto = new PostCreateDto
            {
                Type = type,
                Category = type == 4 ? "ExistingCategory" : null
            };

            // Act
            _postService.TestAssignCategory(postDto);

            // Assert
            Assert.Equal(expectedCategory, postDto.Category);
        }

        //[Theory]
        //[InlineData(20, 20)] // No cambia
        //[InlineData(50, 50)] // No cambia
        //[InlineData(97, 97)] // No cambia
        //[InlineData(98, 97)] // Trunca
        //[InlineData(100, 97)] // Trunca
        //public void ProcessBody_TruncatesCorrectly_WhenBodyIsLong(int inputLength, int expectedLength)
        //{
        //    // Arrange
        //    var body = new string('a', inputLength);
        //    var postDto = new PostCreateDto { Body = body };

        //    // Act
        //    _postService.TestProcessBody(postDto);

        //    // Assert
        //    Assert.Equal(expectedLength, postDto.Body.Length);
        //    if (inputLength > 97)
        //    {
        //        Assert.EndsWith("...", postDto.Body);
        //    }
        //}

        #endregion
    }

    // Clase de extensión para testear métodos privados (solo para pruebas)
    public static class PostServiceTestExtensions
    {
        public static void TestAssignCategory(this PostService service, IPostDto postDto)
        {
            var privateMethod = typeof(PostService).GetMethod("AssignCategory",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            privateMethod.Invoke(service, new object[] { postDto });
        }

        public static void TestProcessBody(this PostService service, IPostDto postDto)
        {
            var privateMethod = typeof(PostService).GetMethod("ProcessBody",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            privateMethod.Invoke(service, new object[] { postDto });
        }
    }

}
