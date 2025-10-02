using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bluewater.App.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Bluewater.IntegrationTests.ExceptionHandling;

public class ExceptionHandlingServiceTests : IAsyncLifetime
{
  private readonly string logDirectory = Path.Combine(Path.GetTempPath(), "BluewaterTests", Guid.NewGuid().ToString("N"));

  public Task InitializeAsync()
  {
    Directory.CreateDirectory(logDirectory);
    return Task.CompletedTask;
  }

  public Task DisposeAsync()
  {
    if (Directory.Exists(logDirectory))
    {
      Directory.Delete(logDirectory, recursive: true);
    }

    return Task.CompletedTask;
  }

  [Fact]
  public async Task Handle_RecordsObservedExceptionInTrace()
  {
    await using var activityTraceService = new ActivityTraceService(logDirectory);
    using var exceptionHandlingService = new ExceptionHandlingService(activityTraceService, NullLogger<ExceptionHandlingService>.Instance);

    var exception = new InvalidOperationException("Handled exception");
    exceptionHandlingService.Handle(exception, "Loading employees");

    string logPath = await activityTraceService.GetCurrentLogPathAsync().ConfigureAwait(false);
    string[] entries = await WaitForLogEntriesAsync(logPath, minimumEntries: 1).ConfigureAwait(false);

    JsonDocument document = JsonDocument.Parse(entries.Last());
    JsonElement metadata = document.RootElement.GetProperty("metadata");

    Assert.Equal("Exception", document.RootElement.GetProperty("commandName").GetString());
    Assert.Equal("Handled", metadata.GetProperty("source").GetString());
    Assert.Equal("Loading employees", metadata.GetProperty("context").GetString());
    Assert.Equal(typeof(InvalidOperationException).FullName, metadata.GetProperty("exceptionType").GetString());
  }

  [Fact]
  public async Task UnobservedTaskException_IsCapturedByTrace()
  {
    await using var activityTraceService = new ActivityTraceService(logDirectory);
    using var exceptionHandlingService = new ExceptionHandlingService(activityTraceService, NullLogger<ExceptionHandlingService>.Instance);

    exceptionHandlingService.Initialize();

    GenerateUnobservedTaskException();

    string logPath = await activityTraceService.GetCurrentLogPathAsync().ConfigureAwait(false);
    string[] entries = await WaitForLogEntriesAsync(logPath, minimumEntries: 1).ConfigureAwait(false);

    JsonDocument document = JsonDocument.Parse(entries.Last());
    JsonElement metadata = document.RootElement.GetProperty("metadata");

    Assert.Equal("Exception", document.RootElement.GetProperty("commandName").GetString());
    Assert.Equal("TaskScheduler", metadata.GetProperty("source").GetString());
    Assert.Equal(typeof(InvalidOperationException).FullName, metadata.GetProperty("exceptionType").GetString());
  }

  private static void GenerateUnobservedTaskException()
  {
    Task task = Task.Run(static () => throw new InvalidOperationException("Background failure"));
    Task.Delay(50).Wait();
    task = null!;

    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
  }

  private static async Task<string[]> WaitForLogEntriesAsync(string logPath, int minimumEntries)
  {
    DateTime endTime = DateTime.UtcNow.AddSeconds(5);
    string[] entries = Array.Empty<string>();

    while (DateTime.UtcNow < endTime)
    {
      if (File.Exists(logPath))
      {
        entries = await File.ReadAllLinesAsync(logPath).ConfigureAwait(false);
        if (entries.Length >= minimumEntries)
        {
          return entries;
        }
      }

      await Task.Delay(100).ConfigureAwait(false);
    }

    throw new TimeoutException($"Failed to find {minimumEntries} log entries in {logPath} within the allotted time.");
  }
}
