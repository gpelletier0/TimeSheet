namespace TimeSheet.Models.Dtos;

public class IdNameDto : BaseDto {
    public string Name { get; init; }

    public override string ToString() {
        return Name;
    }
}