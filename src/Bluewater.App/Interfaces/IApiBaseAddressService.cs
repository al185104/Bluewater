namespace Bluewater.App.Interfaces;

public interface IApiBaseAddressService
{
  string ApiBaseAddress { get; }
  Uri ApiBaseUri { get; }
  bool TryUpdate(string? candidateAddress, out string? validationMessage);
}
