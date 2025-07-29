using AutoMapper;
using Business.Results;
using DataAccess.Data;
using DataAccess.Repositories;
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

        public Task<OperationResult<IEnumerable<PostDto>>> GetAllAsync()
        {
            var posts = _postRepository.GetAll();
            return Task.FromResult(OperationResult<IEnumerable<PostDto>>.Ok(
                _mapper.Map<IEnumerable<PostDto>>(posts)));
        }

        public async Task<OperationResult<PostDto>> GetByIdAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                return OperationResult<PostDto>.NotFound($"Post with ID {id} not found.");

            return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(post));
        }

        public async Task<OperationResult<PostDto>> CreateAsync(PostCreateDto postDto)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(postDto.CustomerId);
                if (customer == null)
                {
                    return OperationResult<PostDto>.NotFound($"Customer with ID {postDto.CustomerId} not found.");
                }

                // Procesar Body según requerimiento
                if (!string.IsNullOrEmpty(postDto.Body) && postDto.Body.Length > 20)
                {
                    postDto.Body = postDto.Body.Length > 97
                        ? postDto.Body.Substring(0, 97) + "..."
                        : postDto.Body;
                }

                // Asignar Category según Type
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

                var post = _mapper.Map<Post>(postDto);
                _postRepository.Create(post);

                return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(post));
            }
            catch (Exception ex)
            {
                return OperationResult<PostDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

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

                    // Procesar Body según requerimiento
                    if (!string.IsNullOrEmpty(postDto.Body) && postDto.Body.Length > 20)
                    {
                        postDto.Body = postDto.Body.Length > 97
                            ? postDto.Body.Substring(0, 97) + "..."
                            : postDto.Body;
                    }

                    // Asignar Category según Type
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

        public async Task<OperationResult<PostDto>> UpdateAsync(int id, PostUpdateDto postDto)
        {
            try
            {
                var originalPost = await _postRepository.GetByIdAsync(id);
                if (originalPost == null)
                {
                    return OperationResult<PostDto>.NotFound($"Post with ID {id} not found.");
                }

                _mapper.Map(postDto, originalPost);
                await _postRepository.UpdateAsync(originalPost);

                return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(originalPost));
            }
            catch (Exception ex)
            {
                return OperationResult<PostDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

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


    }

}
