using Bluewater.Core.UserAggregate.Enum;
using Bluewater.UseCases.Users;

namespace Bluewater.Web.Users;

public record UserRecord(
  Guid Id,
  string Username,
  string PasswordHash,
  Credential Credential,
  Guid? SupervisedGroup,
  bool IsGlobalSupervisor);

public static class UserMapper
{
  public static UserRecord ToRecord(UserDTO dto) =>
    new(dto.Id, dto.Username, dto.PasswordHash, dto.Credential, dto.SupervisedGroup, dto.IsGlobalSupervisor);
}
