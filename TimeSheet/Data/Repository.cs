using System.Diagnostics;
using Mapster;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;

namespace TimeSheet.Data;

public class Repository<TEntity>(IDatabaseService dbService) : IRepository<TEntity>
    where TEntity : BaseEntity, new() {

    public async Task<TDto?> FindAsync<TDto>(int id) where TDto : BaseDto, new() {
        var entity = await dbService.Db.FindAsync<TEntity>(id);

        TDto? dto = null;
        if (entity is not null) {
            dto = entity.Adapt<TDto>();
        }

        return dto;
    }

    public async Task<TDto> FindAsync<TDto>(ISpecification spec) where TDto : BaseDto, new() {
        var query = spec.GetQuery();
        var dto = await dbService.Db.FindWithQueryAsync<TDto>(query.Sql, query.Parameters);
        return dto;
    }

    public async Task<List<TDto>> ListAsync<TDto>() {
        var entities = await dbService.Db.Table<TEntity>().ToListAsync();
        var dtos = entities.Adapt<List<TDto>>();
        return dtos;
    }

    public async Task<List<TDto>> ListAsync<TDto>(ISpecification spec) where TDto : BaseDto, new() {
        var query = spec.GetQuery();
        Debug.WriteLine(query.Sql);
        var dtos = await dbService.Db.QueryAsync<TDto>(query.Sql, query.Parameters);
        return dtos;
    }

    public Task<int> AddAsync<TDto>(TDto dto) {
        var entity = dto.Adapt<TEntity>();
        return dbService.Db.InsertAsync(entity);
    }

    public Task<int> UpdateAsync<TDto>(TDto dto) {
        var entity = dto.Adapt<TEntity>();
        return dbService.Db.UpdateAsync(entity);
    }

    public Task<int> DeleteAsync(int id) {
        return dbService.Db.DeleteAsync<TEntity>(id);
    }
}