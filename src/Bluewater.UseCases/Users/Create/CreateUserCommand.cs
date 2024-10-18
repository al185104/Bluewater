using Ardalis.Result;
using Bluewater.Core.UserAggregate.Enum;
namespace Bluewater.UseCases.Users.Create;

public record CreateUserCommand(string Username, string PasswordHash, Credential? Credential, Guid? SupervisedGroup) : Ardalis.SharedKernel.ICommand<Result<Guid>>;