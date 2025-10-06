namespace TimeSheet.Models.Dtos;

public class ClientsDto : BaseDto {
    public string Name { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
}