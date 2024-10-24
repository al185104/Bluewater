using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;

namespace Bluewater.UseCases.Attendances.List;

internal class ListAttendanceHandler(IRepository<Attendance> _repository) : IQueryHandler<ListAttendanceQuery, Result<IEnumerable<AttendanceDTO>>>
{
  public async Task<Result<IEnumerable<AttendanceDTO>>> Handle(ListAttendanceQuery request, CancellationToken cancellationToken)
  {
    var spec = new AttendanceByEmpIdAndDatesSpec(request.empId, request.startDate, request.endDate);
    var attendances = await _repository.ListAsync(spec, cancellationToken);
    if(attendances == null) return Result<IEnumerable<AttendanceDTO>>.NotFound();

    var result = attendances.Select(s => new AttendanceDTO(s.Id, s.EmployeeId, s.ShiftId, s.TimesheetId, s.LeaveId, s.EntryDate, s.WorkHrs, s.LateHrs, s.UnderHrs, s.IsLocked));
    return Result<IEnumerable<AttendanceDTO>>.Success(result);
  }
}
