using CommunityToolkit.Mvvm.ComponentModel;
using TimeSheet.Interfaces;

namespace TimeSheet.Models;

public partial class CheckName<T> : ObservableObject, ICheckName {
    [ObservableProperty]
    private bool _isChecked;

    public required T Value { get; init; }
    public string Name => Value.ToString() ?? string.Empty;
}