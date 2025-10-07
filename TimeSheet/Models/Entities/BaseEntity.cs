using SQLite;

namespace TimeSheet.Models.Entities;

public abstract class BaseEntity {
    [PrimaryKey] 
    [AutoIncrement] 
    [NotNull]
    public int Id { get; set; }
}