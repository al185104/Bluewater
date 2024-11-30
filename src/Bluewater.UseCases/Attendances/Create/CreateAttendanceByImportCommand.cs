using Ardalis.Result;

namespace Bluewater.UseCases.Attendances.Create;
public record CreateAttendanceByImportCommand(AttendanceImportDTO attendance) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
