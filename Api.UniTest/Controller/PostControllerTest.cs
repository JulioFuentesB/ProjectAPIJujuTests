using API.Controllers;
using Business.Common.DTOs.Post;
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

    public class PostControllerTests
    {
        private readonly Mock<IPostService> _mockPostService;
        private readonly Mock<ILogger<PostController>> _mockLogger;
        private readonly PostController _controller;

        public PostControllerTests()
        {
            _mockPostService = new Mock<IPostService>();
            _mockLogger = new Mock<ILogger<PostController>>();
            _controller = new PostController(_mockPostService.Object, _mockLogger.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfPosts()
        {
            // Arrange
            var posts = new List<PostDto> { new PostDto(), new PostDto() };
            _mockPostService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new OperationResult<IEnumerable<PostDto>>
                {
                    Success = true,
                    Data = posts
                });

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(posts, okResult.Value);
        }

        [Fact]
        public async Task GetAll_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockPostService.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error retrieving all posts")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenPostExists()
        {
            // Arrange
            var post = new PostDto { PostId = 1 };
            _mockPostService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = true,
                    Data = post
                });

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(post, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            _mockPostService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Post not found"
                });

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(new { message = "Post not found" }.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetById_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockPostService.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error retrieving post with ID 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_ReturnsCreatedResult_WhenPostIsCreated()
        {
            // Arrange
            var postDto = new PostCreateDto { Title = "Test" };
            var createdPost = new PostDto { PostId = 1, Title = "Test" };

            _mockPostService.Setup(x => x.CreateAsync(postDto))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = createdPost
                });

            // Act
            var result = await _controller.Create(postDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PostController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(createdPost, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenInputIsInvalid()
        {
            // Arrange
            var postDto = new PostCreateDto();
            _mockPostService.Setup(x => x.CreateAsync(postDto))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Title is required"
                });

            // Act
            var result = await _controller.Create(postDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(new { message = "Title is required" }.ToString(), badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Create_ReturnsNotFound_WhenRelatedEntityDoesNotExist()
        {
            // Arrange
            var postDto = new PostCreateDto { Title = "Test" };
            _mockPostService.Setup(x => x.CreateAsync(postDto))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Category not found"
                });

            // Act
            var result = await _controller.Create(postDto);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(new { message = "Category not found" }.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task Create_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var postDto = new PostCreateDto();
            _mockPostService.Setup(x => x.CreateAsync(postDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Create(postDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error creating post")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region CreateBatch Tests

        [Fact]
        public async Task CreateBatch_ReturnsCreatedResult_WhenPostsAreCreated()
        {
            // Arrange
            var postDtos = new List<PostCreateDto> { new PostCreateDto { Title = "Test1" }, new PostCreateDto { Title = "Test2" } };
            var createdPosts = new List<PostDto> { new PostDto { PostId = 1 }, new PostDto { PostId = 2 } };

            _mockPostService.Setup(x => x.CreateBatchAsync(postDtos))
                .ReturnsAsync(new OperationResult<IEnumerable<PostDto>>
                {
                    Success = true,
                    StatusCode = 201,
                    Data = createdPosts
                });

            // Act
            var result = await _controller.CreateBatch(postDtos);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PostController.GetAll), createdAtActionResult.ActionName);
            Assert.Equal(createdPosts, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateBatch_ReturnsBadRequest_WhenInputIsInvalid()
        {
            // Arrange
            var postDtos = new List<PostCreateDto> { new PostCreateDto() };
            _mockPostService.Setup(x => x.CreateBatchAsync(postDtos))
                .ReturnsAsync(new OperationResult<IEnumerable<PostDto>>
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Title is required"
                });

            // Act
            var result = await _controller.CreateBatch(postDtos);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(new { message = "Title is required" }.ToString(), badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CreateBatch_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var postDtos = new List<PostCreateDto> { new PostCreateDto() };
            _mockPostService.Setup(x => x.CreateBatchAsync(postDtos))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateBatch(postDtos);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error creating posts in batch")),
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
            var postDto = new PostUpdateDto { Title = "Updated" };
            var updatedPost = new PostDto { PostId = 1, Title = "Updated" };

            _mockPostService.Setup(x => x.UpdateAsync(1, postDto))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = true,
                    Data = updatedPost
                });

            // Act
            var result = await _controller.Update(1, postDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedPost, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenInputIsInvalid()
        {
            // Arrange
            var postDto = new PostUpdateDto();
            _mockPostService.Setup(x => x.UpdateAsync(1, postDto))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorMessage = "Title is required"
                });

            // Act
            var result = await _controller.Update(1, postDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(new { message = "Title is required" }.ToString(), badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postDto = new PostUpdateDto { Title = "Test" };
            _mockPostService.Setup(x => x.UpdateAsync(1, postDto))
                .ReturnsAsync(new OperationResult<PostDto>
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Post not found"
                });

            // Act
            var result = await _controller.Update(1, postDto);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(new { message = "Post not found" }.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task Update_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var postDto = new PostUpdateDto();
            _mockPostService.Setup(x => x.UpdateAsync(1, postDto))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Update(1, postDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error updating post with ID 1")),
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
            _mockPostService.Setup(x => x.DeleteAsync(1))
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
        public async Task Delete_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            _mockPostService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(new OperationResult<bool>
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "Post not found"
                });

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(new { message = "Post not found" }.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task Delete_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            _mockPostService.Setup(x => x.DeleteAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error deleting post with ID 1")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion
    }

}
