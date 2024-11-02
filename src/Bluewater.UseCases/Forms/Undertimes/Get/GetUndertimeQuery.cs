using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Undertimes.Get;
public record GetUndertimeQuery(Guid UndertimeId) : IQuery<Result<UndertimeDTO>>;
