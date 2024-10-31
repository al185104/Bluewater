using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Deductions.List;
public record ListDeductionQuery(int? skip, int? take) : IQuery<Result<IEnumerable<DeductionDTO>>>;
