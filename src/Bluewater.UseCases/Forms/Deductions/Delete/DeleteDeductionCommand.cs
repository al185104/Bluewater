using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Deductions.Delete;
public record DeleteDeductionCommand(Guid DeductionId) : ICommand<Result>;
