using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Microsoft.Maui.ApplicationModel;

namespace Bluewater.App.Services;

public sealed class AppUpdaterService : IAppUpdaterService
{
  private const string ManifestPreferenceKey = "AppUpdateManifestUrl";

  private readonly HttpClient _httpClient;

  public AppUpdaterService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public string ManifestUrl => Preferences.Get(ManifestPreferenceKey, string.Empty);

  public bool TryUpdateManifestUrl(string? candidateUrl, out string? validationMessage)
  {
    if (!TryNormalizeUrl(candidateUrl, out Uri? manifestUri))
    {
      validationMessage = "Please enter a valid absolute HTTP or HTTPS URL for version.json.";
      return false;
    }

    Preferences.Set(ManifestPreferenceKey, manifestUri!.ToString());
    validationMessage = null;
    return true;
  }

  public async Task<AppUpdateCheckResult> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
  {
    string manifestUrl = ManifestUrl;
    if (string.IsNullOrWhiteSpace(manifestUrl))
    {
      return new AppUpdateCheckResult
      {
        IsConfigured = false,
        CurrentVersion = GetCurrentVersion(),
        Message = "Updater feed URL is not configured yet."
      };
    }

    using HttpResponseMessage response = await _httpClient.GetAsync(manifestUrl, cancellationToken).ConfigureAwait(false);
    response.EnsureSuccessStatusCode();

    await using Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    AppUpdateManifest? manifest = await JsonSerializer.DeserializeAsync<AppUpdateManifest>(
      responseStream,
      new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
      cancellationToken).ConfigureAwait(false);

    if (manifest is null || string.IsNullOrWhiteSpace(manifest.Version) || string.IsNullOrWhiteSpace(manifest.ZipUrl))
    {
      throw new InvalidOperationException("version.json is missing required values: version and zipUrl.");
    }

    if (!Version.TryParse(manifest.Version.Trim(), out Version? availableVersion))
    {
      throw new InvalidOperationException($"The manifest version '{manifest.Version}' is not a valid version value.");
    }

    Version currentVersion = GetCurrentVersion();
    bool updateAvailable = availableVersion > currentVersion;

    return new AppUpdateCheckResult
    {
      IsConfigured = true,
      IsUpdateAvailable = updateAvailable,
      CurrentVersion = currentVersion,
      AvailableVersion = availableVersion,
      Manifest = manifest,
      Message = updateAvailable
        ? $"Update available: {currentVersion} → {availableVersion}."
        : $"You're up to date on version {currentVersion}."
    };
  }

  public async Task<AppUpdateInstallResult> DownloadAndInstallAsync(AppUpdateManifest manifest, CancellationToken cancellationToken = default)
  {
#if !WINDOWS
    return new AppUpdateInstallResult
    {
      IsSuccess = false,
      RequiresRestart = false,
      Message = "Self-update is currently supported on Windows builds only."
    };
#else
    string appDirectory = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    string executableName = ResolveExecutableName(manifest.EntryExecutable);
    string executablePath = Path.Combine(appDirectory, executableName);

    if (!File.Exists(executablePath))
    {
      return new AppUpdateInstallResult
      {
        IsSuccess = false,
        RequiresRestart = false,
        Message = $"Could not find executable '{executableName}' in {appDirectory}."
      };
    }

    string workDirectory = Path.Combine(Path.GetTempPath(), "BluewaterUpdater", Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(workDirectory);

    string zipPath = Path.Combine(workDirectory, "update.zip");
    string extractDirectory = Path.Combine(workDirectory, "content");
    string scriptPath = Path.Combine(workDirectory, "apply-update.cmd");

    try
    {
      using HttpResponseMessage zipResponse = await _httpClient.GetAsync(manifest.ZipUrl, cancellationToken).ConfigureAwait(false);
      zipResponse.EnsureSuccessStatusCode();

      await using (Stream source = await zipResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
      await using (FileStream destination = File.Create(zipPath))
      {
        await source.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
      }

      ZipFile.ExtractToDirectory(zipPath, extractDirectory, overwriteFiles: true);
      string sourceDirectory = ResolvePackageRoot(extractDirectory);

      string script = BuildUpdateScript(sourceDirectory, appDirectory, executablePath);
      await File.WriteAllTextAsync(scriptPath, script, Encoding.UTF8, cancellationToken).ConfigureAwait(false);

      Process.Start(new ProcessStartInfo
      {
        FileName = "cmd.exe",
        Arguments = $"/c \"{scriptPath}\"",
        WorkingDirectory = workDirectory,
        UseShellExecute = false,
        CreateNoWindow = true
      });

      return new AppUpdateInstallResult
      {
        IsSuccess = true,
        RequiresRestart = true,
        Message = "Update downloaded. Bluewater will now close so the updater can replace files and relaunch."
      };
    }
    catch
    {
      TryDeleteDirectory(workDirectory);
      throw;
    }
#endif
  }

  private static Version GetCurrentVersion()
  {
    if (Version.TryParse(AppInfo.Current.VersionString, out Version? appInfoVersion))
    {
      return appInfoVersion;
    }

    return Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0, 0);
  }

  private static bool TryNormalizeUrl(string? input, out Uri? normalizedUri)
  {
    normalizedUri = null;
    if (string.IsNullOrWhiteSpace(input))
    {
      return false;
    }

    if (!Uri.TryCreate(input.Trim(), UriKind.Absolute, out Uri? parsedUri))
    {
      return false;
    }

    if (parsedUri.Scheme is not ("http" or "https"))
    {
      return false;
    }

    normalizedUri = parsedUri;
    return true;
  }

  private static string ResolveExecutableName(string? configuredEntryExecutable)
  {
    if (!string.IsNullOrWhiteSpace(configuredEntryExecutable))
    {
      return configuredEntryExecutable.Trim();
    }

    using Process currentProcess = Process.GetCurrentProcess();
    string? currentExecutable = currentProcess.MainModule?.FileName;
    if (!string.IsNullOrWhiteSpace(currentExecutable))
    {
      return Path.GetFileName(currentExecutable);
    }

    return "Bluewater.App.exe";
  }

  private static string ResolvePackageRoot(string extractDirectory)
  {
    string[] rootFiles = Directory.GetFiles(extractDirectory);
    string[] rootDirectories = Directory.GetDirectories(extractDirectory);

    if (rootFiles.Length == 0 && rootDirectories.Length == 1)
    {
      return rootDirectories[0];
    }

    return extractDirectory;
  }

  private static string BuildUpdateScript(string sourceDirectory, string targetDirectory, string executablePath)
  {
    return $"@echo off{Environment.NewLine}" +
           "setlocal enableextensions" + Environment.NewLine +
           "timeout /t 2 /nobreak >nul" + Environment.NewLine +
           $"set \"SRC={sourceDirectory}\"{Environment.NewLine}" +
           $"set \"DST={targetDirectory}\"{Environment.NewLine}" +
           $"set \"EXE={executablePath}\"{Environment.NewLine}" +
           "robocopy \"%SRC%\" \"%DST%\" /MIR /R:3 /W:1 /NFL /NDL /NJH /NJS >nul" + Environment.NewLine +
           "if %ERRORLEVEL% GEQ 8 exit /b 1" + Environment.NewLine +
           "start \"\" \"%EXE%\"" + Environment.NewLine +
           "rmdir /s /q \"%SRC%\"" + Environment.NewLine +
           "del \"%~f0\"" + Environment.NewLine;
  }

  private static void TryDeleteDirectory(string directoryPath)
  {
    try
    {
      if (Directory.Exists(directoryPath))
      {
        Directory.Delete(directoryPath, recursive: true);
      }
    }
    catch
    {
      // no-op
    }
  }
}
