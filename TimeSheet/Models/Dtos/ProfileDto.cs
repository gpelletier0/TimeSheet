namespace TimeSheet.Models.Dtos;

public class ProfileDto : BaseDto {

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? WebSite { get; set; }
    public byte[]? Image { get; set; }

}