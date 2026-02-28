using Bluewater.App.Models;
using Microsoft.Maui.Storage;

namespace Bluewater.App.Helpers;

public static class TenantPreferences
{
  private const string SelectedTenantKey = "SelectedTenant";

  public static TenantDto GetSelectedTenant()
  {
    string tenant = Preferences.Get(SelectedTenantKey, TenantDto.Maribago.ToString());
    return Enum.TryParse<TenantDto>(tenant, out TenantDto parsed) ? parsed : TenantDto.Maribago;
  }

  public static void EnsureSelectedTenant()
  {
    string tenant = Preferences.Get(SelectedTenantKey, string.Empty);

    if (string.IsNullOrWhiteSpace(tenant))
    {
      Preferences.Set(SelectedTenantKey, TenantDto.Maribago.ToString());
    }
  }
}
