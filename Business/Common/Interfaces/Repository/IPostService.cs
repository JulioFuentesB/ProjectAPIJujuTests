using Business.Common.DTOs.Post;
using Business.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Common.Interfaces.Repository
{
    public interface IPostService
    {
        Task<OperationResult<IQueryable<PostDto>>> GetAllAsync();
        Task<OperationResult<PostDto>> GetByIdAsync(int id);
        Task<OperationResult<PostDto>> CreateAsync(PostCreateDto postDto);
        Task<OperationResult<IEnumerable<PostDto>>> CreateBatchAsync(IEnumerable<PostCreateDto> postDtos);
        Task<OperationResult<PostDto>> UpdateAsync(int id, PostUpdateDto postDto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }
}

