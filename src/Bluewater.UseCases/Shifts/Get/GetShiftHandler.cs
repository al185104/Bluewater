using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.ShiftAggregate;

namespace Bluewater.UseCases.Shifts.Get;
public class GetShiftHandler(IRepository<Shift> _repository) : IQueryHandler<GetShiftQuery, Result<ShiftDTO>>
{
  public async Task<Result<ShiftDTO>> Handle(GetShiftQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.GetByIdAsync(request.shiftId, cancellationToken);
    if (result == null) return Result.NotFound();

    return new ShiftDTO(result!.Id, result.Name, result.ShiftStartTime, result.ShiftBreakTime, result.ShiftBreakEndTime, result.ShiftEndTime, result.BreakHours);
  }
}
