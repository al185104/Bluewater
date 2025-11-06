using System;
using System.Collections.Generic;
using Bluewater.App.Models;
using Bluewater.App.ViewModels;

namespace Bluewater.App.Views;

public partial class AttendancePage : ContentPage
{
  public AttendancePage(AttendanceViewModel vm)
  {
    InitializeComponent();
    BindingContext = vm;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();

    if (BindingContext is AttendanceViewModel viewModel)
    {
      await viewModel.InitializeAsync();
    }
  }

  private async void OnEditAttendanceClicked(object? sender, EventArgs e)
  {
    if (sender is not Element element)
    {
      return;
    }

    if (element.BindingContext is not EmployeeAttendanceSummary summary)
    {
      return;
    }

    if (Shell.Current is null)
    {
      return;
    }

    var routeParameters = new Dictionary<string, object>
    {
      ["Summary"] = summary
    };

    await Shell.Current.GoToAsync(nameof(AttendanceDetailPage), true, routeParameters);
  }
}
