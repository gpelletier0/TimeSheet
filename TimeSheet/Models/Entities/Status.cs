using SQLite;

namespace TimeSheet.Models.Entities;

[Table("Statuses")]
public class Status : BaseEntity {
    [Unique]
    [NotNull]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(7)]
    public string ColorArgb { get; set; }
}