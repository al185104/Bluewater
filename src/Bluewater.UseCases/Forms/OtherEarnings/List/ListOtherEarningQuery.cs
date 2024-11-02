using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.OtherEarnings.List;
public record ListOtherEarningQuery(int? skip, int? take) : IQuery<Result<IEnumerable<OtherEarningDTO>>>;
