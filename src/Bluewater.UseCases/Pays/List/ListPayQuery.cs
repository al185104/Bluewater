using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Pays.List;
public record ListPayQuery(int? skip, int? take) : IQuery<Result<IEnumerable<PayDTO>>>;
