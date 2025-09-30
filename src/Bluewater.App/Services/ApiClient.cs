using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Bluewater.App.Interfaces;

namespace Bluewater.App.Services;

public class ApiClient(HttpClient httpClient) : IApiClient
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
  {
    using HttpResponseMessage response = await httpClient.GetAsync(requestUri, cancellationToken);

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return default;
    }

    response.EnsureSuccessStatusCode();

    if (IsContentEmpty(response.Content.Headers))
    {
      return default;
    }

    await using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
    return await JsonSerializer.DeserializeAsync<T>(contentStream, JsonOptions, cancellationToken);
  }

  public async Task<TResponse?> PostAsync<TRequest, TResponse>(
    string requestUri,
    TRequest request,
    CancellationToken cancellationToken = default)
  {
    using HttpContent content = CreateJsonContent(request);
    using var message = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
    return await SendAsync<TResponse>(message, cancellationToken);
  }

  public async Task<TResponse?> PutAsync<TRequest, TResponse>(
    string requestUri,
    TRequest request,
    CancellationToken cancellationToken = default)
  {
    using HttpContent content = CreateJsonContent(request);
    using var message = new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content };
    return await SendAsync<TResponse>(message, cancellationToken);
  }

  private async Task<T?> SendAsync<T>(HttpRequestMessage message, CancellationToken cancellationToken)
  {
    using HttpResponseMessage response = await httpClient.SendAsync(message, cancellationToken);

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return default;
    }

    if (response.StatusCode == HttpStatusCode.NoContent)
    {
      return default;
    }

    response.EnsureSuccessStatusCode();

    if (IsContentEmpty(response.Content.Headers))
    {
      return default;
    }

    await using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
    return await JsonSerializer.DeserializeAsync<T>(contentStream, JsonOptions, cancellationToken);
  }

  private static bool IsContentEmpty(HttpContentHeaders headers)
  {
    if (headers?.ContentLength.HasValue == true && headers.ContentLength.Value == 0)
    {
      return true;
    }

    return false;
  }

  private static HttpContent CreateJsonContent<T>(T value)
  {
    string json = JsonSerializer.Serialize(value, JsonOptions);
    return new StringContent(json, Encoding.UTF8, "application/json");
  }
}
