using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Holidays.Update;
public record UpdateHolidayCommand(Guid HolidayId, string NewName, string? Description, DateTime Date, bool IsRegular) : ICommand<Result<HolidayDTO>>;
