using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapster;
using SQLite;
using TimeSheet.Attributes;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;
using EmailAddressAttribute = TimeSheet.Attributes.EmailAddressAttribute;

namespace TimeSheet.ViewModels.Clients;

public partial class ClientViewModel(IRepository<Client> clientRepo) : ObservableValidatorViewModel {

    [ObservableProperty]
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
    private string _name;

    [ObservableProperty]
    [MaxLength(50, ErrorMessage = "Contact name cannot be longer than 50 characters")]
    private string? _contactName;

    [ObservableProperty]
    [EmailAddress]
    [MaxLength(254, ErrorMessage = "Contact email cannot be longer than 254 characters")]
    private string? _contactEmail;

    [ObservableProperty]
    [PhoneNumber]
    [MaxLength(12, ErrorMessage = "Contact phone cannot be longer than 12 characters")]
    private string? _contactPhone;

    [ObservableProperty]
    [MaxLength(500, ErrorMessage = "Note cannot be longer than 500 characters")]
    private string? _note;

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(ClientsDto.Id), out var obj)
            && obj is int id) {
            Id = id;
        }
    }

    protected override async Task OnAppearingAsync() {
        Title = IsNew ? "New Client" : "Edit Client";
        CanDelete = !IsNew;

        await GetClientAsync();
    }

    protected override async Task SaveAsync() {
        if (!await ValidateViewModelAsync()) {
            return;
        }

        var dto = this.Adapt<ClientDto>();

        try {
            if (IsNew) {
                await clientRepo.AddAsync(dto);
            }
            else {
                dto.Id = Id;
                await clientRepo.UpdateAsync(dto);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (SQLiteException e) when (e.Message.StartsWith("UNIQUE constraint failed:")) {
            await Shell.Current.DisplayAlert("Error", "A client with that name already exists", "OK");
        }
    }

    protected override async Task DeleteAsync() {
        if (!await Shell.Current.DisplayAlert("Delete Client", "Are you sure you want to delete this client?", "Yes", "No")) {
            return;
        }

        await clientRepo.DeleteAsync(Id);
        await Shell.Current.GoToAsync("..");
    }

    private async Task GetClientAsync() {
        if (IsNew) {
            return;
        }

        var dto = await clientRepo.FindAsync<ClientDto>(Id);
        if (dto is null) {
            return;
        }

        Name = dto.Name;
        ContactName = dto.ContactName;
        ContactEmail = dto.ContactEmail;
        ContactPhone = dto.ContactPhone;
        Note = dto.Note;
    }
}