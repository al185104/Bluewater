﻿using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class SettingPage : ContentPage
{
  public SettingPage(SettingViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnBindingContextChanged()
  {
    base.OnBindingContextChanged();
    if (BindingContext is SettingViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }
}
