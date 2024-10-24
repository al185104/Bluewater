using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;
using Bluewater.UseCases.Users.Create;

namespace Bluewater.UseCases.Users.Create;
public class CreateUserHandler(IRepository<User> _repository) : ICommandHandler<CreateUserCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
  {
    //string username, string passwordHash, Credential? credential, Guid? supervisedGroup
    var newUser = new User(request.Username, request.PasswordHash, request.Credential, request.SupervisedGroup, request.isGlobalSupervisor);
    var createdItem = await _repository.AddAsync(newUser, cancellationToken);
    return createdItem.Id;
  }
}
