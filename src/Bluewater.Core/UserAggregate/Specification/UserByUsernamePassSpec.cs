using Ardalis.Specification;

namespace Bluewater.Core.UserAggregate.Specifications;
public class UserByUsernamePassSpec : Specification<AppUser>
{
  public UserByUsernamePassSpec(string username, string password)
  {
    Query
        .Where(User => 
        User.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase) &&
        User.PasswordHash.Equals(password, StringComparison.InvariantCultureIgnoreCase));
  }
}
