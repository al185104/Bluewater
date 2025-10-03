using System;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Views;

public partial class OutOfSyncContentView : ContentView
{
  public event EventHandler? DismissRequested;

  public OutOfSyncContentView()
  {
    InitializeComponent();
  }

  private void OnCloseClicked(object sender, EventArgs e)
  {
    DismissRequested?.Invoke(this, EventArgs.Empty);
  }
}
