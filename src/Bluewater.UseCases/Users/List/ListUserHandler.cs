using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;

namespace Bluewater.UseCases.Users.List;

internal class ListUserHandler(IRepository<AppUser> _repository) : IQueryHandler<ListUserQuery, Result<IEnumerable<UserDTO>>>
{
  public async Task<Result<IEnumerable<UserDTO>>> Handle(ListUserQuery request, CancellationToken cancellationToken)
  {
    var result = (await _repository.ListAsync(cancellationToken)).Select(s => new UserDTO(s.Id, s.Username, s.PasswordHash, s.Credential, s.SupervisedGroup, s.IsGlobalSupervisor));
    return Result.Success(result);
  }
}
