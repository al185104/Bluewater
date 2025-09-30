namespace Bluewater.App.Interfaces;

public interface IActivityTraceService
{
  Task LogNavigationAsync(string? fromRoute, string? toRoute, object? metadata = null);

  Task LogCommandAsync(string commandName, object? metadata = null);

  Task<string> GetCurrentLogPathAsync();
}
