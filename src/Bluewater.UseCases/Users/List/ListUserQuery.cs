using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Users.List;
public record ListUserQuery(int? skip, int? take) : IQuery<Result<IEnumerable<UserDTO>>>;
