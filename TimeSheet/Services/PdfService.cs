using CommunityToolkit.Maui.Storage;
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
    IRepository<Timesheet> timesheetRepo,
    IRepository<Profile> profileRepo) : IInvoiceService {

    public async Task Generate(int id) {
        var result = await FolderPicker.Default.PickAsync();
        if (!result.IsSuccessful) {
            return;
        }

        var invoiceData = await LoadInvoiceDataAsync(id);
        if (invoiceData is null) {
            await ShowErrorAsync("Unable to load invoice data.");
            return;
        }

        try {
            await GeneratePdfFileAsync(invoiceData, result.Folder.Path);
            await ShowSuccessAsync(invoiceData.FileName);
        }
        catch (Exception ex) {
            await ShowErrorAsync($"Failed to generate PDF: {ex.Message}");
        }
    }

    private async Task<InvoiceData?> LoadInvoiceDataAsync(int invoiceId) {
        var invoiceDto = await invoiceRepo.FindAsync<InvoiceDto>(invoiceId);
        if (invoiceDto is null) {
            return null;
        }

        var clientDto = await clientRepo.FindAsync<ClientDto>(invoiceDto.ClientId);
        if (clientDto is null) {
            return null;
        }

        var projectIds = invoiceDto.ProjectIdArray.JsonDeserialize<HashSet<int>>();
        if (projectIds is null || projectIds.Count == 0) {
            return null;
        }

        var projectDtos = await projectRepo.FindAllAsync<ProjectDto>(projectIds);
        if (projectDtos is null || projectDtos.Count == 0) {
            return null;
        }

        var timesheetIds = invoiceDto.TimesheetIdArray?.JsonDeserialize<HashSet<int>>();
        if (timesheetIds is null || timesheetIds.Count == 0) {
            return null;
        }

        var timesheetDtos = await timesheetRepo.FindAllAsync<TimesheetDto>(timesheetIds);
        if (timesheetDtos is null || timesheetDtos.Count == 0) {
            return null;
        }

        var profileDto = await profileRepo.FindAsync<ProfileDto>(1) ?? new ProfileDto();

        return new InvoiceData(invoiceDto, clientDto, projectDtos, timesheetDtos, profileDto);
    }

    private static async Task GeneratePdfFileAsync(InvoiceData data, string folderPath) {
        var pdfInvoice = new TextPdfInvoice(data);
        var outputPath = Path.Combine(folderPath, data.FileName);

        await pdfInvoice.GeneratePdfAsync(outputPath);
    }

    private static async Task ShowSuccessAsync(string filename) {
        if (Application.Current?.Windows[0].Page is { } page) {
            await page.DisplayAlert("Success", $"{filename} generated successfully", "OK");
        }
    }

    private static async Task ShowErrorAsync(string message) {
        if (Application.Current?.Windows[0].Page is { } page) {
            await page.DisplayAlert("Error", message, "OK");
        }
    }

}