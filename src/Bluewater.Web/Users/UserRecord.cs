using Bluewater.Core.UserAggregate.Enum;
using Bluewater.UseCases.Users;
using Bluewater.Web.Employees;

namespace Bluewater.Web.Users;

public record UserRecord(
  Guid Id,
  string Username,
  string PasswordHash,
  Credential Credential,
  Guid? SupervisedGroup,
  bool IsGlobalSupervisor,
  EmployeeRecord? Employee = null);

public static class UserMapper
{
  public static UserRecord ToRecord(UserDTO dto) =>
    new(
      dto.Id,
      dto.Username,
      dto.PasswordHash,
      dto.Credential,
      dto.SupervisedGroup,
      dto.IsGlobalSupervisor,
      dto.Employee is null ? null : EmployeeMapper.ToRecord(dto.Employee));
}
