using Ardalis.Result;
using Bluewater.Core.Interfaces;
using Bluewater.Core.UserAggregate.Enum;
using Bluewater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class EmployeeAuthService : IEmployeeAuthService
{
  private readonly AppDbContext _context;
  public EmployeeAuthService(AppDbContext context)
  {
    _context = context;
  }
  public async Task<Result<(string, Credential)>> SignInAsync(string username, string password)
  {
    await Task.Delay(100);
    return Result.Success(("user1", Credential.SuperAdmin));

    // var user = await _context.Users
    //   .FirstOrDefaultAsync(i => i.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase) &&
    //   i.PasswordHash.Equals(password, StringComparison.InvariantCultureIgnoreCase));

    //   if(user != null)
    //     return Result.Success((user.Username,user.Credential));
    //   return Result.NotFound();
  }

  public Task SignOutAsync()
  {
    throw new NotImplementedException();
  }

}
