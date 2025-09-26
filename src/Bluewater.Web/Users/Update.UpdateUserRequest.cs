using System.ComponentModel.DataAnnotations;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.Web.Users;

public class UpdateUserRequest
{
  public const string Route = "/Users";

  [Required]
  public Guid Id { get; set; }

  [Required]
  public string Username { get; set; } = string.Empty;

  [Required]
  public string PasswordHash { get; set; } = string.Empty;

  [Required]
  public Credential Credential { get; set; }

  public Guid? SupervisedGroup { get; set; }

  public bool IsGlobalSupervisor { get; set; }
}
