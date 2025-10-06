namespace TimeSheet.Models.Dtos;

public class TimesheetDto : BaseDto {
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Note { get; set; }
    public int? ProjectId { get; set; }
    public int? StatusId { get; set; }
}