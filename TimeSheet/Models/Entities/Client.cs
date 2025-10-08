using SQLite;

namespace TimeSheet.Models.Entities;

[Table("Clients")]
public class Client : BaseEntity {
    [Unique]
    [NotNull]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string? ContactName { get; set; }

    [MaxLength(12)]
    public string? ContactPhone { get; set; }

    [MaxLength(254)]
    public string? ContactEmail { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}