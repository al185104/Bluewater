using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;

namespace Bluewater.UseCases.Attendances.Get;
/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetAttendanceHandler(IReadRepository<Attendance> _repository) : IQueryHandler<GetAttendanceQuery, Result<AttendanceDTO>>
{
  public async Task<Result<AttendanceDTO>> Handle(GetAttendanceQuery request, CancellationToken cancellationToken)
  {
    var spec = new AttendanceByIdSpec(request.AttendanceId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new AttendanceDTO(entity.Id, entity.EmployeeId, entity.ShiftId, entity.TimesheetId, entity.LeaveId, entity.EntryDate, entity.WorkHrs, entity.LateHrs, entity.UnderHrs, entity.IsLocked); 
  }
}
