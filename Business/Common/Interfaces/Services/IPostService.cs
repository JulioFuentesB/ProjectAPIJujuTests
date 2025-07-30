using Business.Common.DTOs.Post;
using Business.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Common.Interfaces.Services
{
    public interface IPostService
    {
        /// <summary>
        /// Obtiene todas las publicaciones.
        /// </summary>
        /// <returns>Resultado con la lista de publicaciones</returns>
        Task<OperationResult<IEnumerable<PostDto>>> GetAllAsync();

        /// <summary>
        /// Obtiene una publicación por su ID.
        /// </summary>
        /// <param name="id">ID de la publicación</param>
        /// <returns>Resultado con la publicación encontrada</returns>
        Task<OperationResult<PostDto>> GetByIdAsync(int id);

        /// <summary>
        /// Crea una nueva publicación.
        /// </summary>
        /// <param name="postDto">Datos de la nueva publicación</param>
        /// <returns>Resultado con la publicación creada</returns>
        Task<OperationResult<PostDto>> CreateAsync(PostCreateDto postDto);

        /// <summary>
        /// Crea un lote de publicaciones.
        /// </summary>
        /// <param name="postDtos">Lista de publicaciones a crear</param>
        /// <returns>Resultado con las publicaciones creadas</returns>
        Task<OperationResult<IEnumerable<PostDto>>> CreateBatchAsync(IEnumerable<PostCreateDto> postDtos);

        /// <summary>
        /// Actualiza una publicación existente.
        /// </summary>
        /// <param name="id">ID de la publicación a actualizar</param>
        /// <param name="postDto">Datos actualizados de la publicación</param>
        /// <returns>Resultado con la publicación actualizada</returns>
        Task<OperationResult<PostDto>> UpdateAsync(int id, PostUpdateDto postDto);

        /// <summary>
        /// Elimina una publicación por su ID.
        /// </summary>
        /// <param name="id">ID de la publicación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        Task<OperationResult<bool>> DeleteAsync(int id);

    }
}
