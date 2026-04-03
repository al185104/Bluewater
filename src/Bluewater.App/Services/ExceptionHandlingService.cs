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

namespace Bluewater.App.Services;

public sealed class ExceptionHandlingService : IExceptionHandlingService, IDisposable
{
  private readonly IActivityTraceService activityTraceService;
  private readonly ILogger<ExceptionHandlingService>? logger;
  private Popup? activePopup;
  private bool fatalErrorDisplayed;
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
    if (TryGetPresentationException(exception, out PresentationException? presentationException))
    {
      if (presentationException == null)
        return;
      ShowPresentationPopup(presentationException);
      return;
    }

    LogException(source, exception, context, isTerminating);
    _ = CreateDiagnosticArchiveAsync(exception);
    ShowFatalErrorPage(new PresentationException(exception));
  }

  private static bool TryGetPresentationException(Exception exception, out PresentationException? presentationException)
  {
    if (exception is PresentationException directPresentationException)
    {
      presentationException = directPresentationException;
      return true;
    }

    if (exception.InnerException is PresentationException innerPresentation)
    {
      presentationException = innerPresentation;
      return true;
    }

    presentationException = null;
    return false;
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

  private void ShowPresentationPopup(PresentationException presentationException)
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

  private void ShowFatalErrorPage(PresentationException presentationException)
  {
    ArgumentNullException.ThrowIfNull(presentationException);

    IDispatcher? dispatcher = Application.Current?.Dispatcher;

    if (dispatcher is null)
    {
      logger?.LogWarning("Unable to display fatal error page because the dispatcher is unavailable.");
      return;
    }

    void DisplayFatalError()
    {
      if (fatalErrorDisplayed)
      {
        return;
      }

      Window? window = Application.Current?.Windows.FirstOrDefault();
      if (window is null)
      {
        logger?.LogWarning("Unable to display fatal error page because the application window is unavailable.");
        return;
      }

      fatalErrorDisplayed = true;
      activePopup = null;
      window.Page = new FatalErrorPage(presentationException);
    }

    if (dispatcher.IsDispatchRequired)
    {
      dispatcher.Dispatch(DisplayFatalError);
    }
    else
    {
      DisplayFatalError();
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

  private Task CreateDiagnosticArchiveAsync(Exception exception)
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

        string appDataDirectory = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
          "Bluewater",
          "App",
          "Logs");

        if (!Directory.Exists(appDataDirectory))
        {
          logger?.LogWarning("App data directory '{AppDataDirectory}' does not exist; skipping log archive.", appDataDirectory);
          return;
        }

        List<FileInfo> logFiles = Directory
          .EnumerateFiles(appDataDirectory, "*.log")
          .Select(path => new FileInfo(path))
          .Where(file => file.Exists)
          .OrderByDescending(file => file.LastWriteTimeUtc)
          .ToList();

        if (logFiles.Count == 0)
        {
          logger?.LogWarning("No logs found to archive from '{AppDataDirectory}'.", appDataDirectory);
          return;
        }
        string downloadsDirectory = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
          "Downloads",
          "DiagnosticLogs");

        if (string.IsNullOrWhiteSpace(downloadsDirectory))
        {
          logger?.LogWarning("Downloads directory could not be resolved; skipping log archive.");
          return;
        }

        Directory.CreateDirectory(downloadsDirectory);

        string zipFileName = $"DiagnosticLogs_{DateTimeOffset.Now:yyyyMMdd_HHmmss}.zip";
        string destinationPath = Path.Combine(downloadsDirectory, zipFileName);

        if (File.Exists(destinationPath))
        {
          File.Delete(destinationPath);
        }

        using (ZipArchive archive = ZipFile.Open(destinationPath, ZipArchiveMode.Create))
        {
          foreach (FileInfo logFile in logFiles)
          {
            archive.CreateEntryFromFile(logFile.FullName, logFile.Name);
          }

          string exceptionDetails = $"Generated: {DateTimeOffset.Now:O}{Environment.NewLine}{Environment.NewLine}{exception}";
          ZipArchiveEntry exceptionEntry = archive.CreateEntry("exception.txt");
          using Stream entryStream = exceptionEntry.Open();
          using var writer = new StreamWriter(entryStream);
          writer.Write(exceptionDetails);
        }

        logger?.LogInformation("Created diagnostic archive at '{ArchivePath}' from '{AppDataDirectory}'.", destinationPath, appDataDirectory);
      }
      catch (Exception archiveException)
      {
        logger?.LogError(archiveException, "Failed to create diagnostic archive in Downloads.");
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
