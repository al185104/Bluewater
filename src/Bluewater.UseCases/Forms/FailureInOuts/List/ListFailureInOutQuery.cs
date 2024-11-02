using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.FailureInOuts.List;
public record ListFailureInOutQuery(int? skip, int? take) : IQuery<Result<IEnumerable<FailureInOutDTO>>>;
