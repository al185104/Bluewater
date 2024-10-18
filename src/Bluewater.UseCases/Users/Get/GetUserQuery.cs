using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Users.Get;
public record GetUserQuery(Guid? UserId) : IQuery<Result<UserDTO>>;
