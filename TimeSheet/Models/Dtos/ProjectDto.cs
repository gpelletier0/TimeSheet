namespace TimeSheet.Models.Dtos;

public class ProjectDto : BaseDto {
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal HourlyWage { get; set; }
    public int? ClientId { get; set; }
}