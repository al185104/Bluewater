using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.UseCases.Users.Update;
public record UpdateUserCommand(Guid UserId, string Username, string PasswordHash, Credential Credential, Guid? SupervisedGroup) : ICommand<Result<UserDTO>>;
