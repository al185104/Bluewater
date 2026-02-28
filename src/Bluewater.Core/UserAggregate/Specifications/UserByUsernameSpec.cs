using Ardalis.Specification;

namespace Bluewater.Core.UserAggregate.Specifications;

public class UserByUsernameSpec : Specification<AppUser>
{
  public UserByUsernameSpec(string username)
  {
    var normalizedUsername = username.Trim();

    Query.Where(user => user.Username.ToLower() == normalizedUsername.ToLower());
  }
}
