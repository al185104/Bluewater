using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Holidays.Delete;
public record DeleteHolidayCommand(Guid HolidayId) : ICommand<Result>;
