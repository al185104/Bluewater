using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.OtherEarnings.Get;
public record GetOtherEarningQuery(Guid OtherEarningId) : IQuery<Result<OtherEarningDTO>>;
