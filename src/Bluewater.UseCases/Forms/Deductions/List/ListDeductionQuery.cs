using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Deductions.List;
public record ListDeductionQuery(int? skip, int? take) : IQuery<Result<IEnumerable<DeductionDTO>>>;
