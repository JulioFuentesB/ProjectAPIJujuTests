using Business.Results;
using System.Linq;

namespace Business.Common.Interfaces.Services
{
    public interface IBaseService<TEntity, TDto> where TEntity : class, new()
    {
        OperationResult<IQueryable<TDto>> GetAll();
        OperationResult<TDto> GetById(object id);
        OperationResult<TDto> Create(TDto dto);
        OperationResult<TDto> Update(object id, TDto dto);
        OperationResult<bool> Delete(object id);
    }
}
