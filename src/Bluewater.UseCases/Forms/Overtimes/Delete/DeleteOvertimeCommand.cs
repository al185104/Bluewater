using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Overtimes.Delete;
public record DeleteOvertimeCommand(Guid OvertimeId) : ICommand<Result>;
