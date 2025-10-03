using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
