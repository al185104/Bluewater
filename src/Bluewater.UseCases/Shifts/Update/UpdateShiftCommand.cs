using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Shifts.Update;
public record UpdateShiftCommand(Guid ShiftId, string NewName, TimeOnly? start, TimeOnly? breakstart, TimeOnly? breakend, TimeOnly? end, decimal breakhours) : ICommand<Result<ShiftDTO>>;
