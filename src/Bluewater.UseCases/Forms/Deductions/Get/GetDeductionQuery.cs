using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Deductions.Get;
public record GetDeductionQuery(Guid DeductionId) : IQuery<Result<DeductionDTO>>;
