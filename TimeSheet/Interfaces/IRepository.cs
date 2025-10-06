using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;

namespace TimeSheet.Interfaces {
    public interface IRepository<TEntity> where TEntity : BaseEntity {
        Task<TDto?> FindAsync<TDto>(int id) where TDto : BaseDto, new();
        Task<TDto> FindAsync<TDto>(ISpecification spec) where TDto : BaseDto, new();
        Task<List<TDto>> ListAsync<TDto>();
        Task<List<TDto>> ListAsync<TDto>(ISpecification spec) where TDto : BaseDto, new();
        Task<int> AddAsync<TDto>(TDto dto);
        Task<int> UpdateAsync<TDto>(TDto dto);
        Task<int> DeleteAsync(int id);
    }
}