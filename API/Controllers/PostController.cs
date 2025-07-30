using Business.Common.DTOs.Post;
using Business.Common.Interfaces.Services;
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
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostService postService,
            ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        /// <summary>
        /// Recupera todas las publicaciones.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _postService.GetAllAsync();
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all posts");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Recupera una publicación por Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _postService.GetByIdAsync(id);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving post with ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea una nueva publicación.
        /// </summary>
        /// <param name="postDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] PostCreateDto postDto)
        {
            try
            {
                var result = await _postService.CreateAsync(postDto);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return CreatedAtAction(nameof(GetById), new { id = result.Data.PostId }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un lote de publicaciones.
        /// </summary>
        /// <param name="postDtos"></param>
        /// <returns></returns>
        [HttpPost("batch")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBatch([FromBody] IEnumerable<PostCreateDto> postDtos)
        {
            try
            {
                var result = await _postService.CreateBatchAsync(postDtos);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return CreatedAtAction(nameof(GetAll), result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating posts in batch");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza una publicación existente.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] PostUpdateDto postDto)
        {
            try
            {
                var result = await _postService.UpdateAsync(id, postDto);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating post with ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza una publicación existente.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _postService.DeleteAsync(id);

                if (!result.Success)
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting post with ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}

