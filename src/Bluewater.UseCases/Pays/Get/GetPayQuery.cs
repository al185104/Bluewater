using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Pays.Get;
public record GetPayQuery(Guid? PayId) : IQuery<Result<PayDTO>>;
