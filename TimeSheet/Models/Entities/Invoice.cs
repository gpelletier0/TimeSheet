using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using Table = SQLite.TableAttribute;
namespace TimeSheet.Models.Entities;

[Table("Invoices")]
public class Invoice : BaseEntity {
    [Indexed(Unique = true)]
    public string Number { get; set; }

    [Indexed]
    public DateTime IssueDate { get; set; }

    [Indexed]
    public DateTime DueDate { get; set; }

    [Indexed]
    [ForeignKey(nameof(Client))]
    public int? ClientId { get; set; }

    public string ProjectIdArray { get; set; }
    public string TimesheetIdArray { get; set; }

    public string? Comments { get; set; }
    
    [Indexed]
    [ForeignKey(nameof(Status))]
    public int? StatusId { get; set; }
}