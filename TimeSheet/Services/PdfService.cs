using TimeSheet.Documents.Pdf;
using TimeSheet.Extensions;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;

namespace TimeSheet.Services;

public class PdfService(
    IRepository<Invoice> invoiceRepo,
    IRepository<Client> clientRepo,
    IRepository<Project> projectRepo,
    IRepository<Timesheet> timesheetRepo) : IInvoiceService {

    public async Task Generate(int id) {
        var invoiceDto = await invoiceRepo.FindAsync<InvoiceDto>(id);
        if (invoiceDto is null) {
            return;
        }

        var clientDto = await clientRepo.FindAsync<ClientDto>(invoiceDto.ClientId);
        if (clientDto is null) {
            return;
        }

        var projectIds = invoiceDto.ProjectIdArray.JsonDeserialize<HashSet<int>>();
        if (projectIds is null) {
            return;
        }

        var projectDtos = await projectRepo.FindAllAsync<ProjectDto>(projectIds);
        if (projectDtos is null) {
            return;
        }

        var timesheetIds = invoiceDto.TimesheetIdArray?.JsonDeserialize<HashSet<int>>();
        if (timesheetIds is null) {
            return;
        }
        
        var timesheetDtos = await timesheetRepo.FindAllAsync<TimesheetDto>(timesheetIds);
        if (timesheetDtos is null) {
            return;
        }

        var pdfInvoice = new TextPdfInvoice(invoiceDto, clientDto, projectDtos, timesheetDtos);
        var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{invoiceDto.Number}.pdf");

        await pdfInvoice.GeneratePdfAsync(outputPath);
    }
}