using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Deductions.Get;
public record GetDeductionQuery(Guid DeductionId) : IQuery<Result<DeductionDTO>>;
