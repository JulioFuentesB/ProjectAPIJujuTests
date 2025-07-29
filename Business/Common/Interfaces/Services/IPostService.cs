using Business.Common.DTOs.Post;
using Business.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Common.Interfaces.Services
{
    public interface IPostService
    {
        Task<OperationResult<PostDto>> CreateAsync(PostCreateDto postDto);
        Task<OperationResult<IEnumerable<PostDto>>> CreateBatchAsync(IEnumerable<PostCreateDto> postDtos);
        Task<OperationResult<bool>> DeleteAsync(int id);

        //Task<OperationResult<PostDto>> CreateAsync(PostCreateDto postDto);
        //Task<OperationResult<IEnumerable<PostDto>>> CreateBatchAsync(IEnumerable<PostCreateDto> postDtos);
        //Task<OperationResult<bool>> DeleteAsync(int id);

        //Task<OperationResult<IQueryable<PostDto>>> GetAllAsync();
        //Task<OperationResult<PostDto>> GetByIdAsync(int id);
        //Task<OperationResult<PostDto>> UpdateAsync(int id, PostUpdateDto postDto);
        Task<OperationResult<IEnumerable<PostDto>>> GetAllAsync();
        Task<OperationResult<PostDto>> GetByIdAsync(int id);
        Task<OperationResult<PostDto>> UpdateAsync(int id, PostUpdateDto postDto);
    }
}
