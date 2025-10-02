using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace Bluewater.App.Services;

public sealed class ExceptionHandlingService : IExceptionHandlingService, IDisposable
{
  private readonly IActivityTraceService activityTraceService;
  private readonly ILogger<ExceptionHandlingService>? logger;
  private bool isInitialized;
  private IDispatcher? dispatcher;

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

    dispatcher = Application.Current?.Dispatcher;
    if (dispatcher is not null)
    {
      dispatcher.UnhandledException += OnDispatcherUnhandledException;
    }

    isInitialized = true;
  }

  public void Handle(Exception exception, string context)
  {
    if (exception is null)
    {
      throw new ArgumentNullException(nameof(exception));
    }

    LogException("Handled", exception, context);
  }

  private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
  {
    if (e.ExceptionObject is Exception exception)
    {
      LogException("AppDomain", exception, context: null, e.IsTerminating);
    }
    else
    {
      LogMessage("AppDomain", $"Non-exception object encountered: {e.ExceptionObject}");
    }
  }

  private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
  {
    LogException("TaskScheduler", e.Exception, context: null);
    e.SetObserved();
  }

  private void OnDispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
  {
    LogException("Dispatcher", e.Exception, context: null);
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

  public void Dispose()
  {
    AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
    TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

    if (dispatcher is not null)
    {
      dispatcher.UnhandledException -= OnDispatcherUnhandledException;
    }
  }
}
