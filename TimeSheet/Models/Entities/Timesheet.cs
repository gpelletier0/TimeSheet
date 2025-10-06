using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace TimeSheet.Models.Entities {
    [SQLite.Table("Timesheets")]
    public class Timesheet : BaseEntity {
        [Indexed]
        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Indexed]
        [ForeignKey(nameof(TimesheetStatus))]
        public int StatusId { get; set; }

        [Indexed]
        [ForeignKey(nameof(Project))]
        public int? ProjectId { get; set; }
    }
}