using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapster;
using SQLite;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace TimeSheet.ViewModels.Projects;

public partial class ProjectViewModel(IRepository<Project> projectRepo, IRepository<Client> clientRepo) : ObservableValidatorViewModel {

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
    private string _name;

    [ObservableProperty]
    [Range(0, double.MaxValue, ErrorMessage = "Hourly wage must be greater than or equal to 0")]
    private decimal _hourlyWage;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private int? _clientId;

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _clientDtos = [];

    [ObservableProperty]
    private IdNameDto? _selectedClientDto;

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(TimesheetsDto.Id), out var obj)
            && obj is int id) {
            Id = id;
        }
    }

    protected override async Task OnAppearingAsync() {
        Title = IsNew ? "New Project" : "Edit Project";
        CanDelete = !IsNew;

        await LoadPickerAsync();
        await GetProjectAsync();

        SelectedClientDto = ClientDtos.SingleOrDefault(c => c.Id == ClientId);
    }

    protected override async Task SaveAsync() {
        if (!await ValidateViewModelAsync()) {
            return;
        }

        var dto = this.Adapt<ProjectDto>();
        dto.ClientId = SelectedClientDto?.Id;

        try {
            if (IsNew) {
                await projectRepo.AddAsync(dto);
            }
            else {
                dto.Id = Id;
                await projectRepo.UpdateAsync(dto);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (SQLiteException e) when (e.Message.StartsWith("UNIQUE constraint failed:")) {
            await Shell.Current.DisplayAlert("Error", "A project with that name already exists", "OK");
        }
    }

    protected override async Task DeleteAsync() {
        if (!await Shell.Current.DisplayAlert("Delete Project", "Are you sure you want to delete this project?", "Yes", "No")) {
            return;
        }

        await projectRepo.DeleteAsync(Id);
        await Shell.Current.GoToAsync("..");
    }

    private async Task LoadPickerAsync() {
        var clients = await clientRepo.ListAsync<IdNameDto>();
        ClientDtos = new ObservableCollection<IdNameDto>(clients);
    }

    private async Task GetProjectAsync() {
        if (IsNew) {
            return;
        }

        var dto = await projectRepo.FindAsync<ProjectDto>(Id);
        if (dto is null) {
            return;
        }

        Name = dto.Name;
        HourlyWage = dto.HourlyWage;
        Description = dto.Description;
        ClientId = dto.ClientId == 0 ? null : dto.ClientId;
    }
}