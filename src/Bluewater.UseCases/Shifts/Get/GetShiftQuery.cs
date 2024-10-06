using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Shifts.Get;
public record GetShiftQuery(Guid shiftId) : IQuery<Result<ShiftDTO>>;
