using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Shifts.List;
public record ListShiftQuery(int? skip, int? take) : IQuery<Result<IEnumerable<ShiftDTO>>>;
