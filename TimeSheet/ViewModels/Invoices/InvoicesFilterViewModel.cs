using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeSheet.Attributes;
using TimeSheet.Interfaces;
using TimeSheet.Models;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Invoices;

namespace TimeSheet.ViewModels.Invoices;

public partial class InvoicesFilterViewModel(IRepository<Status> statusRepo) : ObservableValidatorViewModel {

    [ObservableProperty]
    private string? _number;

    [ObservableProperty]
    private DateTime? _issueDate;

    [ObservableProperty]
    [DateAfterOrEqual(nameof(IssueDate))]
    private DateTime? _dueDate;

    [ObservableProperty]
    private ObservableCollection<CheckName<IdNameDto>> _statusCheckNames;

    private InvoicesSpec _invoicesSpec;
    private bool _isInitialized;

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(InvoicesSpec), out var obj)
            && obj is InvoicesSpec spec) {
            _invoicesSpec = spec;
        }
    }

    protected override async Task OnAppearingAsync() {
        CanDelete = true;

        if (_isInitialized) {
            return;
        }

        await LoadStatusCheckNamesAsync();
        GetSpec();

        _isInitialized = true;
    }

    protected override async Task SaveAsync() {
        if (!await ValidateViewModelAsync()) {
            return;
        }

        SetSpec();
        var parameters = new ShellNavigationQueryParameters { { nameof(InvoicesSpec), _invoicesSpec } };
        await Shell.Current.GoToAsync(nameof(InvoicesPage), parameters);
    }

    protected override Task DeleteAsync() {
        Number = null;
        IssueDate = null;
        DueDate = null;

        foreach (var statusCheckName in StatusCheckNames) {
            statusCheckName.IsChecked = false;
        }

        return Task.CompletedTask;
    }

    private async Task LoadStatusCheckNamesAsync() {
        var statuses = await statusRepo.ListAsync<IdNameDto>();
        var checkNames = statuses
            .Where(d => !d.Name.Equals("Opened", StringComparison.OrdinalIgnoreCase))
            .Select(d => new CheckName<IdNameDto> { Value = d });

        StatusCheckNames = new ObservableCollection<CheckName<IdNameDto>>(checkNames);
    }

    private void GetSpec() {
        Number = _invoicesSpec.Number;
        IssueDate = _invoicesSpec.IssueDate;
        DueDate = _invoicesSpec.DueDate;

        foreach (var checkName in StatusCheckNames) {
            checkName.IsChecked = _invoicesSpec.StatusIds.Contains(checkName.Value.Id);
        }
    }

    private void SetSpec() {
        _invoicesSpec.Number = Number;
        _invoicesSpec.IssueDate = IssueDate;
        _invoicesSpec.DueDate = DueDate;
        _invoicesSpec.StatusIds = StatusCheckNames
            .Where(c => c.IsChecked)
            .Select(c => c.Value.Id)
            .ToHashSet();
    }
}