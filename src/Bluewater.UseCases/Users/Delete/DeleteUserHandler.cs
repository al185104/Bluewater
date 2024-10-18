using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate;

namespace Bluewater.UseCases.Users.Delete;
public class DeleteUserHandler(IRepository<User> _repository) : ICommandHandler<DeleteUserCommand, Result>
{
  public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
  {
    User? aggregateToDelete = await _repository.GetByIdAsync(request.UserId);
    if (aggregateToDelete == null) return Result.NotFound();
    await _repository.DeleteAsync(aggregateToDelete);
    return Result.Success();
  }
}

