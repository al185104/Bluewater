namespace Bluewater.App.Models;

public sealed class AppUpdateManifest
{
  public string Version { get; set; } = string.Empty;

  public string ZipUrl { get; set; } = string.Empty;

  public string? AppInstallerUrl { get; set; }

  public string? MsixUrl { get; set; }

  public string? EntryExecutable { get; set; }

  public string? ReleaseNotesUrl { get; set; }
}

public sealed class AppUpdateCheckResult
{
  public bool IsConfigured { get; init; }

  public bool IsUpdateAvailable { get; init; }

  public Version CurrentVersion { get; init; } = new(1, 0, 0, 0);

  public Version? AvailableVersion { get; init; }

  public AppUpdateManifest? Manifest { get; init; }

  public string? Message { get; init; }
}

public sealed class AppUpdateInstallResult
{
  public bool IsSuccess { get; init; }

  public bool RequiresRestart { get; init; }

  public string Message { get; init; } = string.Empty;
}
