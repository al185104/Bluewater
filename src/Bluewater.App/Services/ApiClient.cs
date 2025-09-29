using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

  private static bool IsContentEmpty(HttpContentHeaders headers)
  {
    if (headers?.ContentLength.HasValue == true && headers.ContentLength.Value == 0)
    {
      return true;
    }

    return false;
  }
}
