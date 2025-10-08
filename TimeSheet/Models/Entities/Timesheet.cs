using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using Table = SQLite.TableAttribute;

namespace TimeSheet.Models.Entities;

[Table("Timesheets")]
public class Timesheet : BaseEntity {
    [Indexed]
    public DateTime Date { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    [Indexed]
    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; }

    [Indexed]
    [ForeignKey(nameof(Project))]
    public int? ProjectId { get; set; }
}