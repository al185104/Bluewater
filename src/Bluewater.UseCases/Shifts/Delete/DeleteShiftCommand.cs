using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Shifts.Delete;
public record DeleteShiftCommand(Guid ShiftId) : ICommand<Result>;
