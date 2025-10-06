using TimeSheet.Models.Dtos;

namespace TimeSheet.Interfaces;

public class IdNameDto : BaseDto {
    public string Name { get; init; }

    public override string ToString() {
        return Name;
    }
}