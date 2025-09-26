using System.ComponentModel.DataAnnotations;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.Web.Users;

public class CreateUserRequest
{
  public const string Route = "/Users";

  [Required]
  public string Username { get; set; } = string.Empty;

  [Required]
  public string PasswordHash { get; set; } = string.Empty;

  public Credential Credential { get; set; } = Credential.None;

  public Guid? SupervisedGroup { get; set; }

  public bool IsGlobalSupervisor { get; set; }
}
