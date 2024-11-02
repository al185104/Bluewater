using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.UseCases.Forms.FailureInOuts;

namespace Bluewater.UseCases.Forms.FailureInOuts.Get;
public record GetFailureInOutQuery(Guid FailureInOutId) : IQuery<Result<FailureInOutDTO>>;
