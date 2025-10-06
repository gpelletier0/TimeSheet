namespace TimeSheet.Interfaces;

public interface ICheckName {
    bool IsChecked { get; set; }
    string Name { get; }
}