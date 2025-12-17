using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Exceptions;
using Bluewater.App.Interfaces;
using Bluewater.App.Views;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;

namespace Bluewater.App.Services;

public sealed class ExceptionHandlingService : IExceptionHandlingService, IDisposable
{
  private readonly IActivityTraceService activityTraceService;
  private readonly ILogger<ExceptionHandlingService>? logger;
  private Popup? activePopup;
  private bool isInitialized;

  public ExceptionHandlingService(
    IActivityTraceService activityTraceService,
    ILogger<ExceptionHandlingService>? logger = null)
  {
    this.activityTraceService = activityTraceService ?? throw new ArgumentNullException(nameof(activityTraceService));
    this.logger = logger;
  }

  public void Initialize()
  {
    if (isInitialized)
    {
      return;
    }

    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

#if WINDOWS
    if (Microsoft.UI.Xaml.Application.Current is not null)
    {
      Microsoft.UI.Xaml.Application.Current.UnhandledException += OnApplicationUnhandledException;
    }
#endif

    isInitialized = true;
  }

  public void Handle(Exception exception, string context)
  {
    if (exception is null)
    {
      throw new ArgumentNullException(nameof(exception));
    }

    ProcessException("Handled", exception, context);
  }

  private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
  {
    if (e.ExceptionObject is Exception exception)
    {
      ProcessException("AppDomain", exception, context: null, e.IsTerminating);
    }
    else
    {
      LogMessage("AppDomain", $"Non-exception object encountered: {e.ExceptionObject}");
    }
  }

  private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
  {
    ProcessException("TaskScheduler", e.Exception, context: null);
    e.SetObserved();
  }

#if WINDOWS
  private void OnApplicationUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
  {
    ProcessException("Application", e.Exception, context: null);
    e.Handled = true;
  }
#endif

  private void ProcessException(string source, Exception exception, string? context, bool isTerminating = false)
  {
    LogException(source, exception, context, isTerminating);

    PresentationException presentationException = ConvertToPresentationException(exception);
    _ = CopyLatestLogToDesktopAsync();
    ShowOutOfSyncPopup(presentationException);
  }

  private static PresentationException ConvertToPresentationException(Exception exception)
  {
    if (exception is PresentationException presentationException)
    {
      return presentationException;
    }

    return exception.InnerException is PresentationException innerPresentation
      ? innerPresentation
      : new PresentationException(exception);
  }

  private void LogException(string source, Exception exception, string? context, bool isTerminating = false)
  {
    logger?.LogError(exception, "Exception captured from {Source} (Context: {Context})", source, context);

    var metadata = new Dictionary<string, object?>
    {
      ["source"] = source,
      ["context"] = context,
      ["exceptionType"] = exception.GetType().FullName,
      ["message"] = exception.Message,
      ["stackTrace"] = exception.StackTrace,
      ["isTerminating"] = isTerminating,
    };

    if (exception.InnerException is not null)
    {
      metadata["innerException"] = exception.InnerException.ToString();
    }

    _ = WriteTraceAsync("Exception", metadata);
  }

  private void ShowOutOfSyncPopup(PresentationException presentationException)
  {
    ArgumentNullException.ThrowIfNull(presentationException);

    IDispatcher? dispatcher = Application.Current?.Dispatcher;

    if (dispatcher is null)
    {
      logger?.LogWarning("Unable to display error popup because the dispatcher is unavailable.");
      return;
    }

    void DisplayPopup()
    {
      if (Application.Current?.Windows[0].Page is not Page mainPage)
      {
        logger?.LogWarning("Unable to display error popup because the main page is unavailable.");
        return;
      }

      if (activePopup is not null)
      {
        return;
      }

      var popup = new OutOfSyncPopup(presentationException);
      popup.Closed += OnPopupClosed;

      activePopup = popup;
      mainPage.ShowPopup(popup);
    }

    if (dispatcher.IsDispatchRequired)
    {
      dispatcher.Dispatch(DisplayPopup);
    }
    else
    {
      DisplayPopup();
    }
  }

  private void OnPopupClosed(object? sender, EventArgs e)
  {
    if (sender is Popup popup)
    {
      popup.Closed -= OnPopupClosed;
    }
    activePopup = null;
  }

  private void LogMessage(string source, string message)
  {
    logger?.LogError("Unhandled exception notification from {Source}: {Message}", source, message);

    var metadata = new Dictionary<string, object?>
    {
      ["source"] = source,
      ["message"] = message
    };

    _ = WriteTraceAsync("Exception", metadata);
  }

  private Task WriteTraceAsync(string eventName, object metadata)
  {
    return Task.Run(async () =>
    {
      try
      {
        await activityTraceService.LogCommandAsync(eventName, metadata).ConfigureAwait(false);
      }
      catch (Exception loggingException)
      {
        logger?.LogError(loggingException, "Failed to write exception trace for {EventName}", eventName);
      }
    });
  }

  private Task CopyLatestLogToDesktopAsync()
  {
    return Task.Run(() =>
    {
      try
      {
        if (!OperatingSystem.IsWindows())
        {
          logger?.LogDebug("Skipping log archive because Desktop export is only enabled on Windows.");
          return;
        }

        string appDataDirectory = FileSystem.AppDataDirectory;

        if (!Directory.Exists(appDataDirectory))
        {
          logger?.LogWarning("App data directory '{AppDataDirectory}' does not exist; skipping log archive.", appDataDirectory);
          return;
        }

        FileInfo? latestLog = Directory
          .EnumerateFiles(appDataDirectory, "activity-trace-*.log")
          .Select(path => new FileInfo(path))
          .OrderByDescending(file => file.LastWriteTimeUtc)
          .ThenByDescending(file => file.CreationTimeUtc)
          .FirstOrDefault(file => file.Exists);

        if (latestLog is null)
        {
          logger?.LogWarning("No activity trace logs found to archive.");
          return;
        }

        string desktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        if (string.IsNullOrWhiteSpace(desktopDirectory))
        {
          logger?.LogWarning("Desktop directory could not be resolved; skipping log archive.");
          return;
        }

        Directory.CreateDirectory(desktopDirectory);

        string zipFileName = $"{Path.GetFileNameWithoutExtension(latestLog.Name)}-{DateTimeOffset.Now:yyyyMMddHHmmss}.zip";
        string destinationPath = Path.Combine(desktopDirectory, zipFileName);

        if (File.Exists(destinationPath))
        {
          File.Delete(destinationPath);
        }

        using (ZipArchive archive = ZipFile.Open(destinationPath, ZipArchiveMode.Create))
        {
          archive.CreateEntryFromFile(latestLog.FullName, latestLog.Name);
        }

        logger?.LogInformation("Copied latest activity log '{LogFile}' to Desktop as '{ArchivePath}'.", latestLog.FullName, destinationPath);
      }
      catch (Exception archiveException)
      {
        logger?.LogError(archiveException, "Failed to archive the latest activity log to the Desktop.");
      }
    });
  }

  public void Dispose()
  {
    AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
    TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

#if WINDOWS
    if (Microsoft.UI.Xaml.Application.Current is not null)
    {
      Microsoft.UI.Xaml.Application.Current.UnhandledException -= OnApplicationUnhandledException;
    }
#endif
  }
}
