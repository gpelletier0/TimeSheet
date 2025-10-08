namespace TimeSheet.Models.Dtos;

public class InvoicesDto : BaseDto {
    public string Number { get; set; }
    public string ClientName { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? ColorArgb { get; set; }
}