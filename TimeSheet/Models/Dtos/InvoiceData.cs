namespace TimeSheet.Models.Dtos;

public sealed record InvoiceData(
    InvoiceDto Invoice,
    ClientDto Client,
    IEnumerable<ProjectDto> Projects,
    IEnumerable<TimesheetDto> Timesheets,
    ProfileDto Profile) {

    public string FileName => $"{Invoice.Number}.pdf";

}