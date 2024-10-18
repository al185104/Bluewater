using Ardalis.Specification;

namespace Bluewater.Core.UserAggregate.Specifications;
public class UserByIdSpec : Specification<User>
{
  public UserByIdSpec(Guid UserId)
  {
    Query
        .Where(User => User.Id == UserId);
  }
}
