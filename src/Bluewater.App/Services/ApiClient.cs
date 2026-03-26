using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bluewater.App.Exceptions;
using Bluewater.App.Interfaces;

namespace Bluewater.App.Services;

public class ApiClient(HttpClient httpClient, IApiBaseAddressService apiBaseAddressService) : IApiClient
{
  private static readonly Regex GuidRegex = new(
    "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}",
    RegexOptions.Compiled);

  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
  {
    string normalizedRequestUri = NormalizeGuidCasing(requestUri);

    using HttpResponseMessage response = await httpClient
      .GetAsync(BuildRequestUri(normalizedRequestUri), cancellationToken)
      .ConfigureAwait(false);

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return default;
    }

    await EnsureSuccessOrThrowAsync(response, cancellationToken).ConfigureAwait(false);

    if (IsContentEmpty(response.Content.Headers))
    {
      return default;
    }

    await using Stream contentStream = await response.Content
      .ReadAsStreamAsync(cancellationToken)
      .ConfigureAwait(false);
    return await JsonSerializer
      .DeserializeAsync<T>(contentStream, JsonOptions, cancellationToken)
      .ConfigureAwait(false);
  }

  public async Task<TResponse?> PostAsync<TRequest, TResponse>(
    string requestUri,
    TRequest request,
    CancellationToken cancellationToken = default)
  {
    string normalizedRequestUri = NormalizeGuidCasing(requestUri);
    using HttpContent content = CreateJsonContent(request);
    using var message = new HttpRequestMessage(HttpMethod.Post, BuildRequestUri(normalizedRequestUri)) { Content = content };
    return await SendAsync<TResponse>(message, cancellationToken).ConfigureAwait(false);
  }

  public async Task<TResponse?> PutAsync<TRequest, TResponse>(
    string requestUri,
    TRequest request,
    CancellationToken cancellationToken = default)
  {
    string normalizedRequestUri = NormalizeGuidCasing(requestUri);
    using HttpContent content = CreateJsonContent(request);
    using var message = new HttpRequestMessage(HttpMethod.Put, BuildRequestUri(normalizedRequestUri)) { Content = content };
    return await SendAsync<TResponse>(message, cancellationToken).ConfigureAwait(false);
  }

  public async Task<bool> DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
  {
    string normalizedRequestUri = NormalizeGuidCasing(requestUri);
    using var message = new HttpRequestMessage(HttpMethod.Delete, BuildRequestUri(normalizedRequestUri));
    using HttpResponseMessage response = await httpClient
      .SendAsync(message, cancellationToken)
      .ConfigureAwait(false);

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return false;
    }

    await EnsureSuccessOrThrowAsync(response, cancellationToken).ConfigureAwait(false);
    return true;
  }


  private Uri BuildRequestUri(string requestUri)
  {
    if (Uri.TryCreate(requestUri, UriKind.Absolute, out Uri? absoluteUri))
    {
      return absoluteUri;
    }

    return new Uri(apiBaseAddressService.ApiBaseUri, requestUri);
  }

  private static string NormalizeGuidCasing(string requestUri)
  {
    return GuidRegex.Replace(requestUri, match => match.Value.ToUpperInvariant());
  }

  private async Task<T?> SendAsync<T>(HttpRequestMessage message, CancellationToken cancellationToken)
  {
    using HttpResponseMessage response = await httpClient
      .SendAsync(message, cancellationToken)
      .ConfigureAwait(false);

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      return default;
    }

    if (response.StatusCode == HttpStatusCode.NoContent)
    {
      return default;
    }

    await EnsureSuccessOrThrowAsync(response, cancellationToken).ConfigureAwait(false);

    if (IsContentEmpty(response.Content.Headers))
    {
      return default;
    }

    await using Stream contentStream = await response.Content
      .ReadAsStreamAsync(cancellationToken)
      .ConfigureAwait(false);
    return await JsonSerializer
      .DeserializeAsync<T>(contentStream, JsonOptions, cancellationToken)
      .ConfigureAwait(false);
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

  private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, CancellationToken cancellationToken)
  {
    if (response.IsSuccessStatusCode)
    {
      return;
    }

    if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.UnprocessableEntity)
    {
      string? responseContent = await TryReadContentAsync(response.Content, cancellationToken).ConfigureAwait(false);
      string message = BuildValidationMessage(responseContent);
      throw new PresentationException(message, new HttpRequestException(
        $"Request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).",
        null,
        response.StatusCode));
    }

    response.EnsureSuccessStatusCode();
  }

  private static async Task<string?> TryReadContentAsync(HttpContent? content, CancellationToken cancellationToken)
  {
    if (content is null || IsContentEmpty(content.Headers))
    {
      return null;
    }

    return await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
  }

  private static string BuildValidationMessage(string? responseContent)
  {
    IReadOnlyList<string> validationMessages = ExtractValidationMessages(responseContent);

    if (validationMessages.Count == 0)
    {
      return "The request could not be saved because some details are invalid. Please review the required fields and try again.";
    }

    return $"Please review the entered details and try again: {string.Join(" ", validationMessages)}";
  }

  private static IReadOnlyList<string> ExtractValidationMessages(string? responseContent)
  {
    if (string.IsNullOrWhiteSpace(responseContent))
    {
      return Array.Empty<string>();
    }

    try
    {
      using JsonDocument document = JsonDocument.Parse(responseContent);
      List<string> messages = [];

      CollectMessages(document.RootElement, messages);

      return messages
        .Where(message => !string.IsNullOrWhiteSpace(message))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Take(5)
        .ToArray();
    }
    catch (JsonException)
    {
      return Array.Empty<string>();
    }
  }

  private static void CollectMessages(JsonElement element, List<string> messages)
  {
    switch (element.ValueKind)
    {
      case JsonValueKind.Object:
        foreach (JsonProperty property in element.EnumerateObject())
        {
          if (property.NameEquals("errors") || property.NameEquals("Errors"))
          {
            CollectErrorMessages(property.Value, messages);
            continue;
          }

          if (property.NameEquals("message") || property.NameEquals("Message")
            || property.NameEquals("reason") || property.NameEquals("Reason")
            || property.NameEquals("title") || property.NameEquals("Title")
            || property.NameEquals("detail") || property.NameEquals("Detail"))
          {
            AddMessage(property.Value, messages);
          }
        }
        break;

      case JsonValueKind.Array:
        foreach (JsonElement child in element.EnumerateArray())
        {
          AddMessage(child, messages);
        }
        break;
    }
  }

  private static void CollectErrorMessages(JsonElement element, List<string> messages)
  {
    switch (element.ValueKind)
    {
      case JsonValueKind.Object:
        foreach (JsonProperty property in element.EnumerateObject())
        {
          CollectErrorMessages(property.Value, messages);
        }
        break;

      case JsonValueKind.Array:
        foreach (JsonElement child in element.EnumerateArray())
        {
          AddMessage(child, messages);
        }
        break;

      default:
        AddMessage(element, messages);
        break;
    }
  }

  private static void AddMessage(JsonElement element, List<string> messages)
  {
    if (element.ValueKind != JsonValueKind.String)
    {
      return;
    }

    string? value = element.GetString()?.Trim();
    if (string.IsNullOrWhiteSpace(value))
    {
      return;
    }

    messages.Add(value);
  }
}
