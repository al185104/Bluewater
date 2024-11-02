using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Deductions.Delete;
public record DeleteDeductionCommand(Guid DeductionId) : ICommand<Result>;
