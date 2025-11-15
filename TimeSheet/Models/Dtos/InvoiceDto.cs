namespace TimeSheet.Models.Dtos;

public class InvoiceDto : BaseDto {
    public string Number { get; set; }
    public int ClientId { get; set; }
    public string ProjectIdArray { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? TimesheetIdArray { get; set; }
    public string? Comments { get; set; }
    public int StatusId { get; set; }
}