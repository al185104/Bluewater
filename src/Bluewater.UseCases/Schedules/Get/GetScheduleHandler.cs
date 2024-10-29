using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.ScheduleAggregate;
using Bluewater.Core.ScheduleAggregate.Specifications;

namespace Bluewater.UseCases.Schedules.Get;
public class GetScheduleHandler(IRepository<Schedule> _schedRepository, IRepository<Employee> _empRepository) : IQueryHandler<GetScheduleQuery, Result<ScheduleDTO>>
{
  public async Task<Result<ScheduleDTO>> Handle(GetScheduleQuery request, CancellationToken cancellationToken)
  {
    var spec = new ScheduleByIdSpec(request.ScheduleId);
    var schedResult = await _schedRepository.FirstOrDefaultAsync(spec, cancellationToken);
    if (schedResult == null) return Result.NotFound();

    var empResult = await _empRepository.GetByIdAsync(schedResult!.EmployeeId, cancellationToken);
    if (empResult == null) return Result.NotFound();
    
    return new ScheduleDTO(schedResult!.Id, $"{empResult!.LastName}, {empResult!.FirstName}", schedResult.EmployeeId, schedResult.ShiftId, schedResult.ScheduleDate, schedResult.IsDefault);
  }
}
