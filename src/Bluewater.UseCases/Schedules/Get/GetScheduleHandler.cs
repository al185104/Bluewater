using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ScheduleAggregate;

namespace Bluewater.UseCases.Schedules.Get;
public class GetScheduleHandler(IRepository<Schedule> _schedRepository, IRepository<Employee> _empRepository) : IQueryHandler<GetScheduleQuery, Result<ScheduleDTO>>
{
  public async Task<Result<ScheduleDTO>> Handle(GetScheduleQuery request, CancellationToken cancellationToken)
  {
    var schedResult = await _schedRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
    if (schedResult == null) Result.NotFound();

    var empResult = await _empRepository.GetByIdAsync(schedResult!.EmployeeId, cancellationToken);
    if (empResult == null) Result.NotFound();
    
    return new ScheduleDTO(schedResult!.Id, $"{empResult!.LastName}, {empResult!.FirstName}", schedResult.EmployeeId, schedResult.ShiftId, schedResult.ScheduleDate, schedResult.IsDefault);
  }
}
