namespace Bluewater.App.Interfaces;

public interface IApiBaseAddressRecoveryService
{
  Task<bool> TryRecoverAsync(string operationName, Func<Task> probeAsync, Exception? triggeringException = null);
}
