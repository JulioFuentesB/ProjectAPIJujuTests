using AutoMapper;
using Business.Results;
using DataAccess.Data;
using DataAccess.Interfaces;
using global::Business.Common.DTOs.Post;
using global::Business.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{

    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public PostService(
            IPostRepository postRepository,
            ICustomerRepository customerRepository,
            IMapper mapper)
        {
            _postRepository = postRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all posts.
        /// </summary>
        /// <returns></returns>
        public Task<OperationResult<IEnumerable<PostDto>>> GetAllAsync()
        {
            List<Post> posts = _postRepository.GetAll().ToList();
            return Task.FromResult(OperationResult<IEnumerable<PostDto>>.Ok(_mapper.Map<IEnumerable<PostDto>>(posts)));
        }

        /// <summary>
        /// Retrieves a post by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OperationResult<PostDto>> GetByIdAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                return OperationResult<PostDto>.NotFound($"Post with ID {id} not found.");

            return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(post));
        }

        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <param name="postDto"></param>
        /// <returns></returns>
        public async Task<OperationResult<PostDto>> CreateAsync(PostCreateDto postDto)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(postDto.CustomerId);
                if (customer == null)
                {
                    return OperationResult<PostDto>.NotFound($"Customer with ID {postDto.CustomerId} not found.");
                }

                //ProcessBodyAndCategory(postDto);
                //AssignCategory(postDto);
                ProcessBodyAndCategory(postDto);

                var post = _mapper.Map<Post>(postDto);
                _postRepository.Create(post);

                return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(post));
            }
            catch (Exception ex)
            {
                return OperationResult<PostDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Creates a batch of posts.
        /// </summary>
        /// <param name="postDtos"></param>
        /// <returns></returns>
        public async Task<OperationResult<IEnumerable<PostDto>>> CreateBatchAsync(IEnumerable<PostCreateDto> postDtos)
        {
            try
            {
                var posts = new List<Post>();
                var errors = new List<string>();

                // Validación inicial del batch
                if (postDtos == null || !postDtos.Any())
                {
                    return OperationResult<IEnumerable<PostDto>>.BadRequest("No posts provided for batch creation");
                }

                foreach (var postDto in postDtos)
                {
                    // Validar Customer
                    var customer = await _customerRepository.GetByIdAsync(postDto.CustomerId);
                    if (customer == null)
                    {
                        errors.Add($"Customer with ID {postDto.CustomerId} not found for post with title '{postDto.Title}'");
                        continue;
                    }

                    //ProcessBodyAndCategory(postDto);
                    //AssignCategory(postDto);
                    ProcessBodyAndCategory(postDto);

                    // Mapear a entidad
                    var post = _mapper.Map<Post>(postDto);
                    posts.Add(post);
                }

                // Manejar errores de validación
                if (errors.Any())
                {
                    return OperationResult<IEnumerable<PostDto>>.BadRequest(
                        $"Validation errors: {string.Join("; ", errors)}");
                }

                // Crear posts en una sola transacción
                await _postRepository.AddRangeAsync(posts); // Versión async

                // Mapear y retornar resultados
                var result = _mapper.Map<IEnumerable<PostDto>>(posts);
                return OperationResult<IEnumerable<PostDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                // Loggear el error aquí si es necesario
                return OperationResult<IEnumerable<PostDto>>.Fail(
                    $"Error creating posts batch: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Updates an existing post.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postDto"></param>
        /// <returns></returns>
        public async Task<OperationResult<PostDto>> UpdateAsync(int id, PostUpdateDto postDto)
        {
            try
            {
                var originalPost = await _postRepository.GetByIdAsync(id);
                if (originalPost == null)
                {
                    return OperationResult<PostDto>.NotFound($"Post with ID {id} not found.");
                }

                //ProcessBodyAndCategory(postDto);
                //AssignCategory(postDto);

                ProcessBodyAndCategory(postDto);

                _mapper.Map(postDto, originalPost);
                await _postRepository.UpdateAsync(originalPost);

                return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(originalPost));
            }
            catch (Exception ex)
            {
                return OperationResult<PostDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Deletes a post by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var post = await _postRepository.GetByIdAsync(id);
                if (post == null)
                {
                    return OperationResult<bool>.NotFound($"Post with ID {id} not found.");
                }

                _postRepository.Remove(post);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        //// Método reutilizable para procesar Body y Category
        //private void ProcessBodyAndCategory(dynamic postDto)
        //{
        //    // Procesar Body según requerimiento
        //    if (!string.IsNullOrEmpty(postDto.Body) && postDto.Body.Length > 20)
        //    {
        //        postDto.Body = postDto.Body.Length > 97
        //            ? postDto.Body.Substring(0, 97) + "..."
        //            : postDto.Body;
        //    }

        //}

        //// Método para asignar Category según Type
        //private void AssignCategory(dynamic postDto)
        //{
        //    switch (postDto.Type)
        //    {
        //        case 1:
        //            postDto.Category = "Farándula";
        //            break;
        //        case 2:
        //            postDto.Category = "Política";
        //            break;
        //        case 3:
        //            postDto.Category = "Fútbol";
        //            break;
        //        default:
        //            // Si ya tiene un valor asignado, no lo cambiamos
        //            postDto.Category = postDto.Category;
        //            break;
        //    }
        //}

        private void ProcessBodyAndCategory(IPostDto postDto)
        {
            ProcessBody(postDto);
            AssignCategory(postDto);
        }

        /// <summary>
        /// Processes the body of the post to ensure it meets the length requirements.
        /// </summary>
        /// <param name="postDto"></param>
        private void ProcessBody(IPostDto postDto)
        {
            if (!string.IsNullOrEmpty(postDto.Body) && postDto.Body.Length > 20)
            {
                postDto.Body = postDto.Body.Length > 97
                    ? postDto.Body.Substring(0, 97) + "..."
                    : postDto.Body;
            }
        }

        /// <summary>
        /// Assigns a category to the post based on its type.
        /// </summary>
        /// <param name="postDto"></param>
        private void AssignCategory(IPostDto postDto)
        {
            switch (postDto.Type)
            {
                case 1:
                    postDto.Category = "Farándula";
                    break;
                case 2:
                    postDto.Category = "Política";
                    break;
                case 3:
                    postDto.Category = "Fútbol";
                    break;
                default:
                    // Si ya tiene un valor asignado, no lo cambiamos
                    postDto.Category = postDto.Category;
                    break;
            }
        }

    }

}
