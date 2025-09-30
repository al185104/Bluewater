using System.Text.Json;
using Bluewater.App.Models;
using Bluewater.App.Services;
using FluentAssertions;

namespace Bluewater.UnitTests.App;

public sealed class ActivityTraceServiceTests : IDisposable
{
  private readonly string tempDirectory = Path.Combine(Path.GetTempPath(), $"trace-{Guid.NewGuid():N}");

  [Fact]
  public async Task LogNavigationAndCommand_AppendsEntries()
  {
    Directory.CreateDirectory(tempDirectory);

    await using var service = new ActivityTraceService(tempDirectory);

    await service.LogNavigationAsync("//login", "//home", new { Phase = "Navigating" });
    await service.LogCommandAsync("SaveShift", new { Count = 2 });

    string logPath = await service.GetCurrentLogPathAsync();
    logPath.Should().StartWith(tempDirectory);
    File.Exists(logPath).Should().BeTrue();

    string[] lines = await File.ReadAllLinesAsync(logPath);
    lines.Should().HaveCount(2);

    var options = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };

    ActivityTraceEntry? navigationEntry = JsonSerializer.Deserialize<ActivityTraceEntry>(lines[0], options);
    navigationEntry.Should().NotBeNull();
    navigationEntry!.EventType.Should().Be("Navigation");
    navigationEntry.FromRoute.Should().Be("//login");
    navigationEntry.ToRoute.Should().Be("//home");
    navigationEntry.CommandName.Should().BeNull();

    ActivityTraceEntry? commandEntry = JsonSerializer.Deserialize<ActivityTraceEntry>(lines[1], options);
    commandEntry.Should().NotBeNull();
    commandEntry!.EventType.Should().Be("Command");
    commandEntry.CommandName.Should().Be("SaveShift");
    commandEntry.FromRoute.Should().BeNull();
    commandEntry.ToRoute.Should().BeNull();
  }

  public void Dispose()
  {
    if (Directory.Exists(tempDirectory))
    {
      Directory.Delete(tempDirectory, recursive: true);
    }
  }
}
