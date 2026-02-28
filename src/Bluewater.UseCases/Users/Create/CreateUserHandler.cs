using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;
using Bluewater.Core.UserAggregate.Specifications;
using Bluewater.UseCases.Users.Create;

namespace Bluewater.UseCases.Users.Create;
public class CreateUserHandler(IRepository<AppUser> _repository) : ICommandHandler<CreateUserCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
  {
    var existingUser = await _repository.FirstOrDefaultAsync(new UserByUsernameSpec(request.Username), cancellationToken);
    if (existingUser != null)
    {
      return existingUser.Id;
    }

    var newUser = new AppUser(request.Username, request.PasswordHash, request.Credential, request.SupervisedGroup, request.isGlobalSupervisor);
    var createdItem = await _repository.AddAsync(newUser, cancellationToken);
    return createdItem.Id;
  }
}
