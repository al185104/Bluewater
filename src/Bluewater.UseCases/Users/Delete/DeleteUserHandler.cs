using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;

namespace Bluewater.UseCases.Users.Delete;
public class DeleteUserHandler(IRepository<AppUser> _repository) : ICommandHandler<DeleteUserCommand, Result>
{
  public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
  {
    AppUser? aggregateToDelete = await _repository.GetByIdAsync(request.UserId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

