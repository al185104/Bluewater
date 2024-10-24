using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;

namespace Bluewater.UseCases.Attendances.Delete;
public class DeleteAttendanceHandler(IRepository<Attendance> _repository) : ICommandHandler<DeleteAttendanceCommand, Result>
{
  public async Task<Result> Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken)
  {
    Attendance? aggregateToDelete = await _repository.GetByIdAsync(request.AttendanceId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

