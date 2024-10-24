using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Attendances.List;
public record ListAttendanceQuery(int? skip, int? take, Guid empId, DateOnly startDate, DateOnly endDate) : IQuery<Result<IEnumerable<AttendanceDTO>>>;
