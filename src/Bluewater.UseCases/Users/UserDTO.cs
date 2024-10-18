namespace Bluewater.UseCases.Users;
using Bluewater.Core.UserAggregate.Enum;

public record UserDTO()
{
    public Guid Id { get; init; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Credential Credential { get; set; } = Credential.None;
    public Guid? SupervisedGroup { get; set; } = null;

    public UserDTO(Guid id, string username, string passwordHash, Credential credential, Guid? supervisedGroup) : this()
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Credential = credential;
        SupervisedGroup = supervisedGroup;
    }
}