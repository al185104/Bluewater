using Bluewater.Core.UserAggregate.Enum;
using Microsoft.Maui.Storage;

namespace Bluewater.App.Helpers;

public static class LoginSession
{
  public const string CurrentUsernameKey = "Login.CurrentUsername";
  public const string CurrentCredentialKey = "Login.CurrentCredential";
  public const string CurrentUserIdKey = "Login.CurrentUserId";

  public static string CurrentUsername => Preferences.Get(CurrentUsernameKey, string.Empty);
  public static Guid CurrentUserId => Guid.TryParse(Preferences.Get(CurrentUserIdKey, string.Empty), out Guid userId) ? userId : Guid.Empty;

  public static Credential CurrentCredential
  {
    get
    {
      string value = Preferences.Get(CurrentCredentialKey, string.Empty);
      if (int.TryParse(value, out int numericCredential)
        && Enum.IsDefined(typeof(Credential), numericCredential))
      {
        return (Credential)numericCredential;
      }

      if (Enum.TryParse(value, ignoreCase: true, out Credential namedCredential))
      {
        return namedCredential;
      }

      return Credential.None;
    }
  }

  public static bool IsManagerOrAbove => CurrentCredential >= Credential.Manager;

  public static void SetCurrentUser(string username, Credential credential, Guid? userId = null)
  {
    Preferences.Set(CurrentUsernameKey, username);
    Preferences.Set(CurrentCredentialKey, ((int)credential).ToString());
    if (userId.HasValue && userId.Value != Guid.Empty)
    {
      Preferences.Set(CurrentUserIdKey, userId.Value.ToString());
    }
    else
    {
      Preferences.Remove(CurrentUserIdKey);
    }
  }

  public static void ClearCurrentUser()
  {
    Preferences.Remove(CurrentUsernameKey);
    Preferences.Remove(CurrentCredentialKey);
    Preferences.Remove(CurrentUserIdKey);
  }
}
