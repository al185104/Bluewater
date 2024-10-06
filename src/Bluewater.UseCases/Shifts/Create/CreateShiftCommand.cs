using Ardalis.Result;

namespace Bluewater.UseCases.Shifts.Create;
public record CreateShiftCommand(string Name, TimeOnly? ShiftStartTime, TimeOnly? ShiftBreakTime, TimeOnly? ShiftBreakEndTime, TimeOnly? ShiftEndTime, decimal BreakHours) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
