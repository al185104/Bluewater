using Ardalis.SharedKernel;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.Core.UserAggregate;
public class AppUser(string username, string passwordHash, Credential? credential, Guid? supervisedGroup, bool isGlobalSupervisor = false) : EntityBase<Guid>, IAggregateRoot
{
  public string Username { get; private set; } = username;
  public string PasswordHash { get; private set; } = passwordHash;
  public Credential Credential { get; private set; } = credential ?? Credential.None;
  public Guid? SupervisedGroup { get; private set; } = credential >= Credential.Manager ? supervisedGroup : null;
  public bool IsGlobalSupervisor { get; private set; } = isGlobalSupervisor;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Navigation Property
  // public virtual Employee Employee { get; set; } = null!;
  public AppUser() : this(string.Empty, string.Empty, Credential.None, null) { }

  public void UpdateUser(string username, string passwordHash, Credential? credential, Guid? supervisedGroup, bool isGlobalSupervisor)
  {
    Username = username;
    PasswordHash = passwordHash;
    Credential = credential ?? Credential.None;
    SupervisedGroup = credential >= Credential.Manager ? supervisedGroup : null;
    IsGlobalSupervisor = isGlobalSupervisor;

    UpdatedDate = DateTime.Now;
  }
}
