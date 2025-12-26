using System;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.App.Models;

public class CreateUserRequestDto
{
  public const string Route = "Users";

  public string Username { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public Credential Credential { get; set; }
  public Guid? SupervisedGroup { get; set; }
  public bool IsGlobalSupervisor { get; set; }
}

public record CreateUserResponseDto(Guid Id);

public class UpdateUserRequestDto
{
  public const string Route = "Users";

  public Guid Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public Credential Credential { get; set; }
  public Guid? SupervisedGroup { get; set; }
  public bool IsGlobalSupervisor { get; set; }
}

public class UpdateUserResponseDto
{
  public UserRecordDto? User { get; set; }
}
