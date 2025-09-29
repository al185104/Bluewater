namespace Bluewater.App.Interfaces;

public interface IApiClient
{
  Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default);
}
