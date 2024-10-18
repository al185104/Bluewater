using Ardalis.Result;
namespace Bluewater.UseCases.Users.Delete;
public record DeleteUserCommand(Guid UserId) : Ardalis.SharedKernel.ICommand<Result>;