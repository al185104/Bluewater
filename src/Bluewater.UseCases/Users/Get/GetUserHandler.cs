using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;
using Bluewater.Core.UserAggregate.Specifications;

namespace Bluewater.UseCases.Users.Get;

public class GetUserHandler(IRepository<User> _repository) : IQueryHandler<GetUserQuery, Result<UserDTO>>
{
  public async Task<Result<UserDTO>> Handle(GetUserQuery request, CancellationToken cancellationToken)
  {
    var spec = new UserByIdSpec(request.UserId ?? Guid.Empty);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return new UserDTO(entity.Id, entity.Username, entity.PasswordHash, entity.Credential, entity.SupervisedGroup);
  }
}
