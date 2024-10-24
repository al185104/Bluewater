using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;

namespace Bluewater.UseCases.Attendances.Create;

public class CreateAttendanceHandler(IRepository<Attendance> _repository) : ICommandHandler<CreateAttendanceCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
  {
    var newAttendance = new Attendance(request.EmployeeId, request.ShiftId, request.TimesheetId, request.LeaveId, request.EntryDate, request.WorkHrs, request.LateHrs, request.UnderHrs, request.IsLocked);
    var createdItem = await _repository.AddAsync(newAttendance, cancellationToken);
    return createdItem.Id;
  }
}
