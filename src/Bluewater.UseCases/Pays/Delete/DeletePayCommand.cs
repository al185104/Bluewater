using Ardalis.Result;
namespace Bluewater.UseCases.Pays.Delete;
public record DeletePayCommand(Guid PayId) : Ardalis.SharedKernel.ICommand<Result>;