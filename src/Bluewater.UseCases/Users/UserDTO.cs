namespace Bluewater.UseCases.Users;
using Bluewater.Core.UserAggregate.Enum;
using Bluewater.UseCases.Employees;

public record UserDTO()
{
    public Guid Id { get; init; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Credential Credential { get; set; } = Credential.None;
    public Guid? SupervisedGroup { get; set; } = null;
    public bool IsGlobalSupervisor { get; set; } = false;
    public EmployeeDTO? Employee { get; set; }

    public UserDTO(Guid id, string username, string passwordHash, Credential credential, Guid? supervisedGroup, bool isGlobalSupervisor, EmployeeDTO? employee = null) : this()
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Credential = credential;
        SupervisedGroup = supervisedGroup;
        IsGlobalSupervisor = isGlobalSupervisor;
        Employee = employee;
    }
}
