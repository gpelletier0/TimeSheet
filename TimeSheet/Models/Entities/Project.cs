using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using Table = SQLite.TableAttribute;

namespace TimeSheet.Models.Entities;

[Table("Projects")]
public class Project : BaseEntity {
    [Indexed]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public decimal HourlyWage { get; set; }

    [Indexed]
    [ForeignKey(nameof(Client))]
    public int? ClientId { get; set; }
}