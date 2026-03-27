using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface IAppUpdaterService
{
  string ManifestUrl { get; }

  bool TryUpdateManifestUrl(string? candidateUrl, out string? validationMessage);

  Task<AppUpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default);

  Task<AppUpdateInstallResult> DownloadAndInstallAsync(AppUpdateManifest manifest, CancellationToken cancellationToken = default);
}
