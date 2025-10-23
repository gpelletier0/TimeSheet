using System.Diagnostics;
using Mapster;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;

namespace TimeSheet.Data;

public class Repository<TEntity>(IDatabaseService dbService) : IRepository<TEntity>
    where TEntity : BaseEntity, new() {

    public string? GetTableName() {
        var mapping = dbService.Db.GetMapping(typeof(TEntity));
        return mapping?.TableName;
    }

    public int FirstIdOrDefault(string name) {
        var tableName = GetTableName();
        if (tableName == null) {
            return 0;
        }

        var id = dbService.Db.ExecuteScalar<int?>($"SELECT Id FROM {tableName} WHERE Name = ? LIMIT 1", name);

        return id ?? 0;
    }

    public T Get<T>(ISpecification spec) {
        var query = spec.GetQuery();
        var result = dbService.Db.ExecuteScalar<T>(query.Sql, query.Parameters);
        return result;
    }

    public List<T> GetAll<T>(ISpecification spec) {
        var query = spec.GetQuery();
        var result = dbService.Db.QueryScalars<T>(query.Sql, query.Parameters);
        return result;
    }

    public async Task<TDto?> FindAsync<TDto>(int id) where TDto : BaseDto, new() {
        var entity = await dbService.DbAsync.FindAsync<TEntity>(id);

        TDto? dto = null;
        if (entity is not null) {
            dto = entity.Adapt<TDto>();
        }

        return dto;
    }

    public async Task<TDto?> FindAsync<TDto>(ISpecification spec) where TDto : BaseDto, new() {
        var query = spec.GetQuery();
        var dto = await dbService.DbAsync.FindWithQueryAsync<TDto>(query.Sql, query.Parameters);
        return dto;
    }

    public async Task<List<TDto>?> FindAllAsync<TDto>(IEnumerable<int> ids) where TDto : BaseDto, new() {
        var entities = await dbService.DbAsync
            .Table<TEntity>()
            .Where(e => ids.Contains(e.Id))
            .ToListAsync();

        var dtos = entities.Adapt<List<TDto>>();
        return dtos;
    }

    public async Task<List<TDto>> ListAsync<TDto>() {
        var entities = await dbService.DbAsync.Table<TEntity>().ToListAsync();
        var dtos = entities.Adapt<List<TDto>>();
        return dtos;
    }

    public async Task<List<TDto>> ListAsync<TDto>(ISpecification spec) where TDto : BaseDto, new() {
        var query = spec.GetQuery();
        var dtos = await dbService.DbAsync.QueryAsync<TDto>(query.Sql, query.Parameters);
        return dtos;
    }

    public Task<int> AddAsync<TDto>(TDto dto) {
        var entity = dto.Adapt<TEntity>();
        return dbService.DbAsync.InsertAsync(entity);
    }

    public Task<int> UpdateAsync<TDto>(TDto dto) {
        var entity = dto.Adapt<TEntity>();
        return dbService.DbAsync.UpdateAsync(entity);
    }

    public Task<int> DeleteAsync(int id) {
        return dbService.DbAsync.DeleteAsync<TEntity>(id);
    }
}