using SQLite;

namespace TimeSheet.Models.Entities;

public class TimesheetStatus : BaseEntity {
    [Unique]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(7)]
    public string ColorArgb { get; set; }
}