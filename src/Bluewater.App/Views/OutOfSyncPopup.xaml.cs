using System;
using Bluewater.App.Exceptions;
using CommunityToolkit.Maui.Views;

namespace Bluewater.App.Views;

public partial class OutOfSyncPopup : Popup
{
  public OutOfSyncPopup(PresentationException exception)
  {
    ArgumentNullException.ThrowIfNull(exception);

    InitializeComponent();
    BindingContext = exception;
  }

  private async void OnDismissRequested(object? sender, EventArgs e)
  {
    await CloseAsync();
  }
}
