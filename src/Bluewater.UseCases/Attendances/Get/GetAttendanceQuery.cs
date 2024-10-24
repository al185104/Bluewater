using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Attendances.Get;
public record GetAttendanceQuery(Guid AttendanceId) : IQuery<Result<AttendanceDTO>>;
