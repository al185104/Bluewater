using Ardalis.Result;
using Bluewater.Core.UserAggregate.Enum;
namespace Bluewater.UseCases.Users.Create;

public record CreateUserCommand(string Username, string PasswordHash, Credential? Credential, Guid? SupervisedGroup, bool isGlobalSupervisor) : Ardalis.SharedKernel.ICommand<Result<Guid>>;