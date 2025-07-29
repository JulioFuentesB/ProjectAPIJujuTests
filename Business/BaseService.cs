using AutoMapper;
using Business.Common.Interfaces.Services;
using DataAccess;
using global::Business.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
namespace Business
{

    public class BaseService<TEntity, TDto> where TEntity : class, new()
    {
        protected readonly BaseModel<TEntity> _baseModel;
        protected readonly IMapper _mapper;

        public BaseService(BaseModel<TEntity> baseModel, IMapper mapper)
        {
            _baseModel = baseModel;
            _mapper = mapper;
        }

        public virtual OperationResult<IQueryable<TDto>> GetAll()
        {
            try
            {
                var entities = _baseModel.GetAll;
                var dtos = _mapper.ProjectTo<TDto>(entities);
                return OperationResult<IQueryable<TDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return OperationResult<IQueryable<TDto>>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        public virtual OperationResult<TDto> GetById(object id)
        {
            try
            {
                var entity = _baseModel.FindById(id);
                if (entity == null)
                    return OperationResult<TDto>.NotFound($"Entity with ID {id} not found.");

                return OperationResult<TDto>.Ok(_mapper.Map<TDto>(entity));
            }
            catch (Exception ex)
            {
                return OperationResult<TDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        public virtual OperationResult<TEntity> Create(TEntity entity)
        {
            try
            {
                
                var createdEntity = _baseModel.Create(entity);
                return OperationResult<TEntity>.Ok(_mapper.Map<TEntity>(createdEntity));
            }
            catch (Exception ex)
            {
                return OperationResult<TEntity>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        public virtual OperationResult<TDto> Update(object id, TDto dto)
        {
            try
            {
                var originalEntity = _baseModel.FindById(id);
                if (originalEntity == null)
                    return OperationResult<TDto>.NotFound($"Entity with ID {id} not found.");

                var editedEntity = _mapper.Map<TEntity>(dto);
                var updatedEntity = _baseModel.Update(editedEntity, originalEntity, out bool changed);

                return OperationResult<TDto>.Ok(_mapper.Map<TDto>(updatedEntity));
            }
            catch (Exception ex)
            {
                return OperationResult<TDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        public virtual OperationResult<bool> Delete(object id)
        {
            try
            {
                var entity = _baseModel.FindById(id);
                if (entity == null)
                    return OperationResult<bool>.NotFound($"Entity with ID {id} not found.");

                _baseModel.Delete(entity);
                return OperationResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
            }
        }
    }

}
