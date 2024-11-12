using Ardalis.Result;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.Core.Interfaces;

public interface IEmployeeAuthService
{
  public Task<Result<(string, Credential)>> SignInAsync(string username, string password);
  public Task SignOutAsync();
  
}
