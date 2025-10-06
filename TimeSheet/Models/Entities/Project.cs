using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace TimeSheet.Models.Entities {
    [SQLite.Table("Projects")]
    public class Project : BaseEntity {
        [Indexed(Unique = true)]
        [MaxLength(100), Unique]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public decimal HourlyWage { get; set; }

        [Indexed]
        [ForeignKey(nameof(Client))]
        public int? ClientId { get; set; }
    }
}