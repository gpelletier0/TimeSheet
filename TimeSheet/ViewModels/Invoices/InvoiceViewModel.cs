using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;

namespace TimeSheet.ViewModels.Invoices;

public partial class InvoiceViewModel(
    IRepository<Invoice> invoiceRepo,
    IRepository<Client> clientRepo,
    IRepository<Project> projectRepo) : ObservableValidatorViewModel {

    private const string InvoicePrefix = "INV";

    [ReadOnly(true)]
    [ObservableProperty]
    private string _number;

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _clientDtos = [];

    [ObservableProperty]
    private IdNameDto? _selectedClientDto;

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _projectDtos = [];

    [ObservableProperty]
    private List<int> _projectIds = [];

    [ObservableProperty]
    private DateTime _issueDate;

    [ObservableProperty]
    private DateTime _dueDate;

    [ObservableProperty]
    private List<int> _timesheeIds = [];

    [ObservableProperty]
    private string? _comments;

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(BaseDto.Id), out var obj)
            && obj is int id) {
            Id = id;
        }
    }

    protected override async Task OnAppearingAsync() {
        Title = IsNew ? "New Invoice" : "Edit Invoice";
        CanDelete = !IsNew;

        await LoadClientsPickerAsync();
        await GetInvoiceAsync();
    }

    private async Task LoadClientsPickerAsync() {
        var clientDtos = await clientRepo.ListAsync<IdNameDto>();
        ClientDtos = new ObservableCollection<IdNameDto>(clientDtos);
    }

    private async Task LoadProjectsPickerAsync(int clientId) {
        var projects = await projectRepo.ListAsync<IdNameDto>(new ProjectsSpec { ClientId = clientId });
        ProjectDtos = new ObservableCollection<IdNameDto>(projects);
    }

    private async Task GetInvoiceAsync() {
        if (IsNew) {
            Number = GetInvoiceNumber();
            IssueDate = DateTime.UtcNow;
            return;
        }

        var invoiceDto = await invoiceRepo.FindAsync<InvoiceDto>(Id);
        if (invoiceDto is not null) {
            
            var clientDto = await clientRepo.FindAsync<ClientDto>(invoiceDto.ClientId);
            if (clientDto is null) {
                return;
            }
            
            Number = invoiceDto.Number;
            SelectedClientDto = ClientDtos.SingleOrDefault(c => c.Id == clientDto.Id);
            ProjectIds = invoiceDto.ProjectIdArray.Split(',').Select(int.Parse).ToList();
            IssueDate = invoiceDto.IssueDate;
            DueDate = invoiceDto.DueDate;
            Comments = invoiceDto.Comments;
            TimesheeIds = invoiceDto.TimesheetIdArray.Split(',').Select(int.Parse).ToList();

            await LoadProjectsPickerAsync(invoiceDto.Id);
        }
    }

    private string GetInvoiceNumber() {
        var currentDate = DateTime.UtcNow;
        var invoiceName = $"{InvoicePrefix}-{currentDate.Year}-{currentDate.Month}";
        var number = GetNextSequenceNumber(invoiceName);

        return $"{invoiceName}-{number:000}";
    }

    private int GetNextSequenceNumber(string name) {
        var spec = new SelectMaxSpec {
            ColumnName = nameof(Invoice.Number),
            Start = -3,
            DataType = SqlDataType.Integer,
            Increment = 1,
            TableName = invoiceRepo.GetTableName()!,
            Pattern = $"{name}%"
        };

        var result = invoiceRepo.Get<int>(spec);
        return result;
    }

    async partial void OnSelectedClientDtoChanged(IdNameDto? value) {
        try {
            if (value == null) {
                ProjectDtos = [];
                return;
            }

            await LoadProjectsPickerAsync(value.Id);
        }
        catch (Exception e) {
            Debug.WriteLine(e);
        }
    }
}