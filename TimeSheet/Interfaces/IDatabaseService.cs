using SQLite;

namespace TimeSheet.Interfaces;

public interface IDatabaseService {
    public SQLiteAsyncConnection DbAsync { get; }
    public SQLiteConnectionWithLock Db { get; }
    public Task InitializeAsync();
}