using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeSheet.Models.Dtos;

[INotifyPropertyChanged]
public partial class InvoiceTimesheetDto : BaseDto {
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? ProjectName { get; set; }
    public decimal? ProjectHourlyWage { get; set; }
    public string? ColorArgb { get; set; }

    [ObservableProperty]
    private bool _isChecked;
}