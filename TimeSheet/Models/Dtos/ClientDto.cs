namespace TimeSheet.Models.Dtos;

public class ClientDto : BaseDto {
    public string Name { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Note { get; set; }
}