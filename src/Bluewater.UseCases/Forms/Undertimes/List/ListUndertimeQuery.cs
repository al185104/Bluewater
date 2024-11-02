using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Undertimes.List;
public record ListUndertimeQuery(int? skip, int? take) : IQuery<Result<IEnumerable<UndertimeDTO>>>;
