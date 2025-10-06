namespace TimeSheet.Models.Dtos;

public class ProjectsDto : BaseDto {
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal HourlyWage { get; set; }
    public string? ClientName { get; set; }
}