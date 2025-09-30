namespace Bluewater.App.Interfaces;

public interface IApiClient
{
  Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default);
  Task<TResponse?> PostAsync<TRequest, TResponse>(
    string requestUri,
    TRequest request,
    CancellationToken cancellationToken = default);
  Task<TResponse?> PutAsync<TRequest, TResponse>(
    string requestUri,
    TRequest request,
    CancellationToken cancellationToken = default);
  Task<bool> DeleteAsync(string requestUri, CancellationToken cancellationToken = default);
}
