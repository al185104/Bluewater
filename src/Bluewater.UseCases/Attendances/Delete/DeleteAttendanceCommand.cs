using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Attendances.Delete;
public record DeleteAttendanceCommand(Guid AttendanceId) : ICommand<Result>;
