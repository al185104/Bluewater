using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;

namespace Bluewater.UseCases.Users.Update;
public class UpdateUserHandler(IRepository<AppUser> _repository) : ICommandHandler<UpdateUserCommand, Result<UserDTO>>
{
  public async Task<Result<UserDTO>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
  {
    var existingUser = await _repository.GetByIdAsync(request.UserId, cancellationToken);
    if (existingUser == null)
    {
      return Result.NotFound();
    }

    existingUser.UpdateUser(request.Username, request.PasswordHash, request.Credential, request.SupervisedGroup, request.IsGlobalSupervisor);

    await _repository.UpdateAsync(existingUser, cancellationToken);

    return Result.Success(new UserDTO(existingUser.Id, existingUser.Username, existingUser.PasswordHash, existingUser.Credential, existingUser.SupervisedGroup, existingUser.IsGlobalSupervisor));
  }
}
