using SQLite;

namespace TimeSheet.Interfaces;

public interface IDatabaseService {
    public SQLiteAsyncConnection Db { get; }
    public Task InitializeAsync();
}