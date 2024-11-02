using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Forms.Undertimes.Delete;
public record DeleteUndertimeCommand(Guid UndertimeId) : ICommand<Result>;
