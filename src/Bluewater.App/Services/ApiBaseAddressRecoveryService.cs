using Bluewater.App.Interfaces;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Services;

public sealed class ApiBaseAddressRecoveryService(IApiBaseAddressService apiBaseAddressService) : IApiBaseAddressRecoveryService
{
  public async Task<bool> TryRecoverAsync(string operationName, Func<Task> probeAsync, Exception? triggeringException = null)
  {
    ArgumentNullException.ThrowIfNull(operationName);
    ArgumentNullException.ThrowIfNull(probeAsync);

    while (true)
    {
      bool shouldPrompt = await ShouldPromptForNewAddressAsync(operationName, triggeringException).ConfigureAwait(false);
      if (!shouldPrompt)
      {
        return false;
      }

      bool hasValidInput = await PromptForValidAddressAsync().ConfigureAwait(false);
      if (!hasValidInput)
      {
        return false;
      }

      try
      {
        await probeAsync().ConfigureAwait(false);
        return true;
      }
      catch (Exception)
      {
        triggeringException = null;
      }
    }
  }

  private async Task<bool> ShouldPromptForNewAddressAsync(string operationName, Exception? triggeringException)
  {
    string currentApiAddress = apiBaseAddressService.ApiBaseAddress;
    string message = triggeringException is null
      ? $"Unable to complete {operationName} using API address:\n{currentApiAddress}\n\nDo you want to enter a new API base address?"
      : $"Unable to complete {operationName}. The configured API address might be incorrect:\n{currentApiAddress}\n\nDo you want to enter a new API base address?";

    Page? page = GetCurrentPage();
    if (page is null)
    {
      return false;
    }

    return await MainThread.InvokeOnMainThreadAsync(() =>
      page.DisplayAlert("API Connection", message, "Update Address", "Cancel")).ConfigureAwait(false);
  }

  private async Task<bool> PromptForValidAddressAsync()
  {
    while (true)
    {
      Page? page = GetCurrentPage();
      if (page is null)
      {
        return false;
      }

      string? candidateAddress = await MainThread.InvokeOnMainThreadAsync(() =>
        page.DisplayPromptAsync(
          "API Base Address",
          "Enter the API base URL (http/https).",
          "Try Address",
          "Cancel",
          initialValue: apiBaseAddressService.ApiBaseAddress,
          keyboard: Keyboard.Url)).ConfigureAwait(false);

      if (candidateAddress is null)
      {
        return false;
      }

      if (apiBaseAddressService.TryUpdate(candidateAddress, out string? validationMessage))
      {
        return true;
      }

      await MainThread.InvokeOnMainThreadAsync(() =>
        page.DisplayAlert("Invalid API Address", validationMessage ?? "Please enter a valid API base URL.", "OK")).ConfigureAwait(false);
    }
  }

  private static Page? GetCurrentPage()
  {
    if (Shell.Current?.CurrentPage is Page shellPage)
    {
      return shellPage;
    }

    Window? window = Application.Current?.Windows.FirstOrDefault();
    return window?.Page;
  }
}
