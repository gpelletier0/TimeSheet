using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;

namespace TimeSheet.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity {
    string? GetTableName();
    int FirstIdOrDefault(string name);
    T Get<T>(ISpecification spec);
    List<T> GetAll<T>(ISpecification spec);
    Task<TDto?> FindAsync<TDto>(int id) where TDto : BaseDto, new();
    Task<List<T>> ListAsync<T>();
    Task<List<TDto>> ListAsync<TDto>(ISpecification spec) where TDto : BaseDto, new();
    Task<int> AddAsync<TDto>(TDto dto);
    Task<int> UpdateAsync<TDto>(TDto dto);
    Task<int> DeleteAsync(int id);
}