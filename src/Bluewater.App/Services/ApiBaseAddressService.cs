using Bluewater.App.Interfaces;

namespace Bluewater.App.Services;

public sealed class ApiBaseAddressService : IApiBaseAddressService
{
  private const string PreferenceKey = "ApiBaseAddress";
  private const string DefaultApiBaseAddress = "https://localhost:57679/";
  private readonly object _syncRoot = new();

  private Uri _apiBaseUri;

  public ApiBaseAddressService()
  {
    string storedAddress = Preferences.Get(PreferenceKey, DefaultApiBaseAddress);
    _apiBaseUri = TryNormalize(storedAddress, out Uri? parsedAddress)
      ? parsedAddress!
      : new Uri(DefaultApiBaseAddress, UriKind.Absolute);
  }

  public string ApiBaseAddress
  {
    get
    {
      lock (_syncRoot)
      {
        return _apiBaseUri.ToString();
      }
    }
  }

  public Uri ApiBaseUri
  {
    get
    {
      lock (_syncRoot)
      {
        return _apiBaseUri;
      }
    }
  }

  public bool TryUpdate(string? candidateAddress, out string? validationMessage)
  {
    if (!TryNormalize(candidateAddress, out Uri? parsedAddress))
    {
      validationMessage = "Please enter a valid absolute HTTP or HTTPS URL, like https://api.example.com/.";
      return false;
    }

    lock (_syncRoot)
    {
      _apiBaseUri = parsedAddress!;
      Preferences.Set(PreferenceKey, _apiBaseUri.ToString());
    }

    validationMessage = null;
    return true;
  }

  private static bool TryNormalize(string? input, out Uri? normalizedUri)
  {
    normalizedUri = null;

    if (string.IsNullOrWhiteSpace(input))
    {
      return false;
    }

    if (!Uri.TryCreate(input.Trim(), UriKind.Absolute, out Uri? parsedUri))
    {
      return false;
    }

    if (parsedUri.Scheme is not ("http" or "https"))
    {
      return false;
    }

    string normalized = parsedUri.ToString();
    if (!normalized.EndsWith('/', StringComparison.Ordinal))
    {
      normalized += "/";
    }

    normalizedUri = new Uri(normalized, UriKind.Absolute);
    return true;
  }
}
