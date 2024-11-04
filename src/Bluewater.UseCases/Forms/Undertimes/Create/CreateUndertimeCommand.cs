using Ardalis.Result;
namespace Bluewater.UseCases.Forms.Undertimes.Create;
public record CreateUndertimeCommand(Guid empId, decimal? inclusiveTime, string? reason, DateOnly? date) : Ardalis.SharedKernel.ICommand<Result<Guid>>;