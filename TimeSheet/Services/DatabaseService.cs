using SQLite;
using TimeSheet.Interfaces;
using TimeSheet.Models.Entities;

namespace TimeSheet.Services;

public class DatabaseService : IDatabaseService {

    private const SQLiteOpenFlags Flags = SQLiteOpenFlags.ReadWrite |
                                          SQLiteOpenFlags.Create |
                                          SQLiteOpenFlags.SharedCache;

    public SQLiteAsyncConnection DbAsync { get; }
    public SQLiteConnectionWithLock Db => DbAsync.GetConnection();

    public DatabaseService(string databaseName) {
        var dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, databaseName);
        DbAsync = new SQLiteAsyncConnection(dbPath, Flags, false);

        Task.Run(InitializeAsync).GetAwaiter().GetResult();
    }

    public async Task InitializeAsync() {
        await DbAsync.ExecuteAsync("PRAGMA foreign_keys = ON;");

        await CreateClientsTable();
        await CreateProjectsTable();
        await CreateStatusesTable();
        await CreateTimesheetsTable();
        await CreateInvoicesTable();
        await CreateProfilesTable();
    }

    private async Task CreateClientsTable() {
        await DbAsync.CreateTableAsync<Client>();
    }

    private async Task CreateProjectsTable() {
        await DbAsync.ExecuteAsync("""
                                   CREATE TABLE IF NOT EXISTS Projects (
                                       Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                       Name TEXT(50) NOT NULL,
                                       Description TEXT(500) NULL,
                                       HourlyWage REAL NOT NULL,
                                       ClientId INTEGER NULL,
                                       FOREIGN KEY(ClientId) REFERENCES Clients(Id) ON DELETE SET NULL
                                   );
                                   """);

        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Projects_Name ON Projects (Name)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Projects_ClientId ON Projects (ClientId)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Projects_ClientId_Id ON Projects (ClientId, Id)");
    }

    private async Task CreateStatusesTable() {
        await DbAsync.CreateTableAsync<Status>();

        var statuses = new Status[] {
            new() { Id = 1, Name = "Opened", ColorArgb = "#FFD700" },
            new() { Id = 2, Name = "Invoiced", ColorArgb = "#B7410E" },
            new() { Id = 3, Name = "Paid", ColorArgb = "#7BB369" },
            new() { Id = 4, Name = "Voided", ColorArgb = "#001829" }
        };

        const string upsertSql = """
                                 INSERT INTO Statuses (Id, Name, ColorArgb)
                                 VALUES (?, ?, ?)
                                 ON CONFLICT(Id) DO 
                                 UPDATE SET
                                     Name = excluded.Name,
                                     ColorArgb = excluded.ColorArgb
                                 WHERE Name != excluded.Name 
                                    OR ColorArgb != excluded.ColorArgb
                                 """;

        await DbAsync.RunInTransactionAsync((s) => {
            foreach (var status in statuses) {
                s.Execute(upsertSql, status.Id, status.Name, status.ColorArgb);
            }
        });
    }

    private async Task CreateTimesheetsTable() {
        await DbAsync.ExecuteAsync("""
                                         CREATE TABLE IF NOT EXISTS Timesheets (
                                             Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                             Date DATE NOT NULL,
                                             StartTime TIME NOT NULL,
                                             EndTime TIME NOT NULL,
                                             Note TEXT(500) NULL,
                                             StatusId INTEGER NULL,
                                             ProjectId INTEGER NULL,
                                             FOREIGN KEY(StatusId) REFERENCES Statuses(Id) ON DELETE SET NULL,
                                             FOREIGN KEY(ProjectId) REFERENCES Projects(Id) ON DELETE SET NULL
                                         );
                                   """);

        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Timesheets_Date ON Timesheets (Date)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Timesheets_StatusId ON Timesheets (StatusId)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Timesheets_ProjectId ON Timesheets (ProjectId)");
    }

    private async Task CreateInvoicesTable() {
        await DbAsync.ExecuteAsync("""
                                   CREATE TABLE IF NOT EXISTS Invoices (
                                       Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                       Number TEXT NOT NULL,
                                       IssueDate DATE NOT NULL,
                                       DueDate DATE NOT NULL,
                                       ClientId INTEGER NULL,
                                       ProjectIdArray TEXT,
                                       TimesheetIdArray TEXT,
                                       Comments TEXT NULL,
                                       StatusId INTEGER NULL,
                                       FOREIGN KEY(ClientId) REFERENCES Clients(Id) ON DELETE SET NULL ,
                                       FOREIGN KEY(StatusId) REFERENCES Statuses(Id) ON DELETE SET NULL
                                   );
                                   """);

        await DbAsync.ExecuteAsync("CREATE UNIQUE INDEX IF NOT EXISTS Invoices_Number ON Invoices (Number)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Invoices_IssueDate ON Invoices (IssueDate)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Invoices_DueDate ON Invoices (DueDate)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Invoices_ClientId ON Invoices (ClientId)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Invoices_StatusId ON Invoices (StatusId)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Invoices_ClientId_StatusId ON Invoices (ClientId, StatusId)");
        await DbAsync.ExecuteAsync("CREATE INDEX IF NOT EXISTS Invoices_Status_DueDate ON Invoices(StatusId, DueDate)");
    }

    private async Task CreateProfilesTable() {
        await DbAsync.CreateTableAsync<Profile>();
    }

}