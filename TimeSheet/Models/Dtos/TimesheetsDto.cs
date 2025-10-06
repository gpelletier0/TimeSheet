namespace TimeSheet.Models.Dtos;

public class TimesheetsDto : BaseDto {
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? ProjectName { get; set; }
    public decimal? ProjectHourlyWage { get; set; }
    public string? StatusColorArgb { get; set; }
}