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
        /// Obtiene todas las publicaciones.
        /// </summary>
        /// <returns>Resultado con la lista de publicaciones</returns>
        public Task<OperationResult<IEnumerable<PostDto>>> GetAllAsync()
        {
            List<Post> posts = _postRepository.GetAll.ToList();
            return Task.FromResult(
                OperationResult<IEnumerable<PostDto>>.Ok(
                    _mapper.Map<IEnumerable<PostDto>>(posts)));
        }

        /// <summary>
        /// Obtiene una publicación por su ID.
        /// </summary>
        /// <param name="id">ID de la publicación</param>
        /// <returns>Resultado con la publicación encontrada</returns>
        public async Task<OperationResult<PostDto>> GetByIdAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
                return OperationResult<PostDto>.NotFound($"No se encontró la publicación con ID {id}.");

            return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(post));
        }

        /// <summary>
        /// Crea una nueva publicación.
        /// </summary>
        /// <param name="postDto">Datos de la nueva publicación</param>
        /// <returns>Resultado con la publicación creada</returns>
        public async Task<OperationResult<PostDto>> CreateAsync(PostCreateDto postDto)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(postDto.CustomerId);

                if (customer == null)
                {
                    return OperationResult<PostDto>.NotFound($"No se encontró el cliente con ID {postDto.CustomerId}.");
                }

                ProcesarCuerpoYCategoria(postDto);

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
        /// Crea un lote de publicaciones.
        /// </summary>
        /// <param name="postDtos">Lista de publicaciones a crear</param>
        /// <returns>Resultado con las publicaciones creadas</returns>
        public async Task<OperationResult<IEnumerable<PostDto>>> CreateBatchAsync(IEnumerable<PostCreateDto> postDtos)
        {
            try
            {
                var posts = new List<Post>();
                var errores = new List<string>();

                // Validación inicial del lote
                if (postDtos == null || !postDtos.Any())
                {
                    return OperationResult<IEnumerable<PostDto>>.BadRequest("No se proporcionaron publicaciones para crear el lote");
                }

                foreach (var postDto in postDtos)
                {
                    // Validar Cliente
                    var customer = await _customerRepository.GetByIdAsync(postDto.CustomerId);

                    if (customer == null)
                    {
                        errores.Add($"No se encontró el cliente con ID {postDto.CustomerId} para la publicación con título '{postDto.Title}'");
                        continue;
                    }

                    ProcesarCuerpoYCategoria(postDto);

                    // Mapear a entidad
                    var post = _mapper.Map<Post>(postDto);
                    posts.Add(post);
                }

                // Manejar errores de validación
                if (errores.Any())
                {
                    return OperationResult<IEnumerable<PostDto>>.BadRequest(
                        $"Errores de validación: {string.Join("; ", errores)}");
                }

                // Crear publicaciones en una sola transacción
                await _postRepository.AddRangeAsync(posts);

                // Mapear y retornar resultados
                var resultado = _mapper.Map<IEnumerable<PostDto>>(posts);
                return OperationResult<IEnumerable<PostDto>>.Ok(resultado);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<PostDto>>.Fail(
                    $"Error al crear el lote de publicaciones: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Actualiza una publicación existente.
        /// </summary>
        /// <param name="id">ID de la publicación a actualizar</param>
        /// <param name="postDto">Datos actualizados de la publicación</param>
        /// <returns>Resultado con la publicación actualizada</returns>
        public async Task<OperationResult<PostDto>> UpdateAsync(int id, PostUpdateDto postDto)
        {
            try
            {
                var publicacionOriginal = await _postRepository.GetByIdAsync(id);

                if (publicacionOriginal == null)
                {
                    return OperationResult<PostDto>.NotFound($"No se encontró la publicación con ID {id}.");
                }

                ProcesarCuerpoYCategoria(postDto);

                _mapper.Map(postDto, publicacionOriginal);
                await _postRepository.UpdateAsync(publicacionOriginal);

                return OperationResult<PostDto>.Ok(_mapper.Map<PostDto>(publicacionOriginal));
            }
            catch (Exception ex)
            {
                return OperationResult<PostDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Elimina una publicación por su ID.
        /// </summary>
        /// <param name="id">ID de la publicación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                var post = await _postRepository.GetByIdAsync(id);

                if (post == null)
                {
                    return OperationResult<bool>.NotFound($"No se encontró la publicación con ID {id}.");
                }

                _postRepository.Remove(post);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Procesa el cuerpo y asigna una categoría a la publicación basada en su tipo.
        /// </summary>
        /// <param name="postDto">DTO de la publicación</param>
        private void ProcesarCuerpoYCategoria(IPostDto postDto)
        {
            ProcesarCuerpo(postDto);
            AsignarCategoria(postDto);
        }

        /// <summary>
        /// Procesa el cuerpo de la publicación para asegurar que cumple con los requisitos de longitud.
        /// </summary>
        /// <param name="postDto">DTO de la publicación</param>
        private void ProcesarCuerpo(IPostDto postDto)
        {
            if (!string.IsNullOrEmpty(postDto.Body) && postDto.Body.Length > 20)
            {
                postDto.Body = postDto.Body.Length > 97
                    ? postDto.Body.Substring(0, 97) + "..."
                    : postDto.Body;
            }
        }

        /// <summary>
        /// Asigna una categoría a la publicación basada en su tipo.
        /// </summary>
        /// <param name="postDto">DTO de la publicación</param>
        private void AsignarCategoria(IPostDto postDto)
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
