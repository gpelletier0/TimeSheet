using SQLite;

namespace TimeSheet.Models.Entities;

[Table("Profiles")]
public class Profile : BaseEntity {

    [NotNull] public string FirstName { get; set; }
    [NotNull] public string LastName { get; set; }
    public string? Address { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    [NotNull] public string Phone { get; set; }
    [NotNull] public string Email { get; set; }
    public string? WebSite { get; set; }
    public byte[]? Image { get; set; }

}