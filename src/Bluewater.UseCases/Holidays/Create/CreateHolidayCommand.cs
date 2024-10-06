using Ardalis.Result;

namespace Bluewater.UseCases.Holidays.Create;

public record CreateHolidayCommand(string Name, string? Description, DateTime Date, bool IsRegular) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
